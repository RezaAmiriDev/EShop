using ClassLibrary.ViewModel;
using Common.Pagination;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace EShope.Pages.Customer
{
    [Authorize]
    //[AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly SettingWeb _settingWeb;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpClientFactory httpClientFactory, SettingWeb settingWeb, ILogger<IndexModel> logger)
        {
            _httpClient = httpClientFactory;
            _settingWeb = settingWeb;
            _logger = logger;
        }

        // helper 
        private HttpClient CreateApiClient()
        {
            var client = _httpClient.CreateClient(_settingWeb.ClinetName);
            var token = User.FindFirst(_settingWeb.TokenName);
            if (token != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(_settingWeb.TokenType, token.Value);
            }
            return client;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public List<CusProDto> Customers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
        {
            if (!User.Identity?.IsAuthenticated ?? true) 
                return Challenge();

            var tokenClaim = User.FindFirst(_settingWeb.TokenName);
            if (tokenClaim == null)
            {
                await HttpContext.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            try
            {
                var client = CreateApiClient();

                // تضمین مقادیر منطقی
                if (PageNumber <= 0) PageNumber = 1;
                if (PageSize <= 0) PageSize = 10;

                // ساختِ بدنهٔ درخواست مطابق ساختار مورد انتظار سرور
                var requestBody = new PagedRequest<CusProDto>
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    StartIndex = (PageNumber - 1) * PageSize,
                    Data = new CusProDto
                    {
                        NationalCode = string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim(),
                        Name = (string?)null // اگر خواستی فیلتر نام‌هم اضافه کنی
                    }
                };

                HttpResponseMessage response;
                try
                {
                    response = await client.PostAsJsonAsync("api/Customer/paged", requestBody, cancellationToken);
                }
                catch (HttpRequestException hex)
                {
                    _logger.LogError(hex, "Unable to reach API for paged customers");
                    ModelState.AddModelError(string.Empty, "خطا در ارتباط با سرویس. لطفاً اتصال اینترنت یا وضعیت سرویس را بررسی کنید.");
                    Customers = new List<CusProDto>();
                    return Page();
                }
                catch (TaskCanceledException tcex) when (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogError(tcex, "Paged request to API timed out");
                    ModelState.AddModelError(string.Empty, "درخواست به سرویس پاسخ نداد (Timeout). لطفاً مجدداً تلاش کنید.");
                    Customers = new List<CusProDto>();
                    return Page();
                }

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await HttpContext.SignOutAsync();
                        return RedirectToPage("/Account/Login", new { area = "Identity" });
                    }

                    var text = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Paged API returned {Status}: {Text}", response.StatusCode, text);

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        ModelState.AddModelError(string.Empty, "موردی برای نمایش یافت نشد.");
                    else
                        ModelState.AddModelError(string.Empty, "خطا در دریافت اطلاعات صفحه‌بندی");

                    Customers = new List<CusProDto>();
                    return Page();
                }

                // روی سرور انتظار داریم PagedResponse<List<CusProDto>> برگردد
                var paged = await response.Content.ReadFromJsonAsync<PagedResult<List<CusProDto>>>(cancellationToken: cancellationToken);
                if (paged != null)
                {
                    Customers = paged.Data ?? new List<CusProDto>();
                    PageNumber = paged.PageNumber > 0 ? paged.PageNumber : PageNumber;
                    PageSize = paged.PageSize > 0 ? paged.PageSize : PageSize;
                    TotalPages = paged.TotalPages > 0 ? paged.TotalPages : 1;
                    TotalRecords = paged.TotalRecords;
                }
                else
                {
                    Customers = new List<CusProDto>();
                    TotalPages = 1;
                    TotalRecords = 0;
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Customer Index (paged)");
                ModelState.AddModelError(string.Empty, "خطای داخلی رخ داد. لطفاً بعداً تلاش کنید.");
                Customers = new List<CusProDto>();
                return Page();
            }
        }

        // Handler برای گرفتن جزئیات یک مشتری به صورت JSON — کلاینت (JS) این handler را صدا می‌زند
        public async Task<IActionResult> OnGetDetailsAsync(Guid id)
        {

            if (id == Guid.Empty) return BadRequest();
            if (!User.Identity?.IsAuthenticated ?? true) return Unauthorized();

            try
            {
                var client = CreateApiClient();
                var response = await client.GetAsync($"api/Customer/{id}");
                if (response.IsSuccessStatusCode)
                {
                    // خواندن پاسخ به صورت string برای دیباگ
                    var jsonString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API Response for customer {CustomerId}: {Response}", id, jsonString);
                    var dto = await response.Content.ReadFromJsonAsync<CusProDto>();
                    return new JsonResult(dto, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNamingPolicy = null // استفاده از PascalCase
                    });
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Customer {CustomerId} not found", id);
                    return NotFound();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access to customer {CustomerId}", id);
                    return Unauthorized();
                }

                return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer details {Id}", id);
                return StatusCode(500);
            }
        }

        // Handler حذف — فرم حذف داخل Index آن را POST می‌کند
        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (id == Guid.Empty) return BadRequest();
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                await HttpContext.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            try
            {
                var client = CreateApiClient();
                var res = await client.DeleteAsync($"api/Customer/{id}");
                if (res.IsSuccessStatusCode)
                {
                    // حذف موفق — بازگشت به خودِ صفحه تا لیست تازه شود
                    return RedirectToPage();
                }
                if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ModelState.AddModelError(string.Empty, "مشتری مورد نظر پیدا نشد.");
                    return Page();
                }
                if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("SignOut", "Account");
                }

                // برای خطاهای دیگر پیام را از بدنه بخوانیم اگر ممکن است
                var text = await res.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"خطا در حذف: {text}");
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "خطای سرور هنگام حذف");
                return Page();
            }
        }
    }
}
