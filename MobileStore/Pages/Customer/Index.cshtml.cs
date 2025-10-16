using ClassLibrary.ViewModel;
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

        public  IndexModel(IHttpClientFactory httpClientFactory , SettingWeb settingWeb , ILogger<IndexModel> logger)
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
            if(token != null)
            {
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue(_settingWeb.TokenType , token.Value);
            }
            return client;
        }

        public List<CusProDto> Customers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                // به شکل امن درخواست ورود (Challenge) یا RedirectToPage
                return Challenge();
            }

            try
            {
                var client = CreateApiClient();

                // اگر توکن وجود ندارد، بهتر SignOut و ریدایرکت به لاگین انجام شود
                var tokenClaim = User.FindFirst(_settingWeb.TokenName);
                if (tokenClaim == null)
                {
                    await HttpContext.SignOutAsync(); // پاک کردن احراز هویت محلی
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                HttpResponseMessage response;

                try
                {
                    response = await client.GetAsync("api/Customer");
                }
                catch(HttpRequestException hex)
                {
                    _logger.LogError(hex , "Unable to reach API at {BaseAddress}" , _settingWeb.BaseAddress);
                    ModelState.AddModelError(string.Empty, "خطا در ارتباط با سرویس. لطفاً اتصال اینترنت یا وضعیت سرویس را بررسی کنید.");
                    Customers = new List<CusProDto>();
                    return Page();
                }
                catch(TaskCanceledException tcex)
                {
                    _logger.LogError(tcex, "Request to API timed out");
                    ModelState.AddModelError(string.Empty, "درخواست به سرویس پاسخ نداد (Timeout). لطفاً مجدداً تلاش کنید.");
                    Customers = new List<CusProDto>();
                    return Page();
                }

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // توکن نامعتبر یا منقضی شده — پاک کن و به صفحه لاگین ببر
                        await HttpContext.SignOutAsync();
                        return RedirectToPage("/Account/Login", new { area = "Identity" });
                    }
                    // وضعیت 404 یعنی داده‌ای نیست — این را نشان بده ولی خطا نیست
                    if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        _logger.LogInformation("GET api/Customer returned 404 (no data).");
                        ModelState.AddModelError(string.Empty, "اطلاعاتی برای نمایش وجود ندارد.");
                        Customers = new List<CusProDto>();
                        return Page();
                    }
                    // وضعیت 400 (BadRequest) یا 500 (Server Error) — تلاش برای خواندن متن خطا
                    string errorText = string.Empty;
                    try
                    {
                        errorText = await response.Content.ReadAsStringAsync();
                    }
                    catch { /* ignore */ }

                    _logger.LogWarning("API returned non-success for GET api/Customer: {Status} - {Text}", (int)response.StatusCode, errorText);

                    // نمایش پیام مناسب بر اساس status code
                    if ((int)response.StatusCode >= 500)
                        ModelState.AddModelError(string.Empty, "خطای داخلی سرور رخ داده است. لطفاً بعداً تلاش کنید.");
                    else if ((int)response.StatusCode == 400)
                        ModelState.AddModelError(string.Empty, $"درخواست نامعتبر: {(!string.IsNullOrWhiteSpace(errorText) ? errorText : response.ReasonPhrase)}");
                    else
                        ModelState.AddModelError(string.Empty, $"خطا در دریافت اطلاعات: {(int)response.StatusCode} {response.ReasonPhrase}");

                    Customers = new List<CusProDto>();
                    return Page();
                }

                try
                {
                    Customers = await response.Content.ReadFromJsonAsync<List<CusProDto>>() ?? new List<CusProDto>();
                    return Page();
                }
                catch (System.Text.Json.JsonException jex)
                {
                    _logger.LogError(jex, "Failed to deserialize api/Customer response");
                    ModelState.AddModelError(string.Empty, "مشکل در پردازش داده‌های برگشتی از سرویس. لطفاً با پشتیبانی تماس بگیرید.");
                    Customers = new List<CusProDto>();
                    return Page();
                }
               
            }
            catch (Exception ex)
            {
                // اگر خطای غیرمنتظره‌ای پیش آمد، لاگ کن و به کاربر پیام بده (بدون ریدایرکت)
                _logger.LogError(ex, "Unexpected error in Customer Index");
                ModelState.AddModelError(string.Empty, "خطای داخلی رخ داد. لطفاً بعداً تلاش کنید.");
                Customers = new List<CusProDto>();
                return Page();
            }
        }

            //var client = _httpClient.CreateClient(_settingWeb.ClinetName);
            //var token = User.FindFirst(_settingWeb.TokenName);
            //if (token == null) return RedirectToAction("SignOut", "Account");
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_settingWeb.TokenType, token.Value);

            //var response = await client.GetAsync("api/Customer");
            //if (!response.IsSuccessStatusCode)
            //{
            //    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //        return RedirectToAction("SignOut", "Account");
            //    // برای خطای دیگر می‌تونی لاگ کنی یا صفحه خطا برگردونی
            //    return RedirectToAction("ErrorPage", "Home");
            //}

            //Customers = await response.Content.ReadFromJsonAsync<List<CusProDto>>() ?? new List<CusProDto>();
            //return Page();

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
                    var dto = await response.Content.ReadFromJsonAsync<CusProDto>();
                    return new JsonResult(dto);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) return Unauthorized();

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
                if(res.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ModelState.AddModelError(string.Empty, "مشتری مورد نظر پیدا نشد.");
                    return Page();
                }
                if(res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("SignOut", "Account");
                }

                // برای خطاهای دیگر پیام را از بدنه بخوانیم اگر ممکن است
                var text = await res.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"خطا در حذف: {text}");
                return Page();
            }catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "خطای سرور هنگام حذف");
                return Page();
            }
        }
    }
}
