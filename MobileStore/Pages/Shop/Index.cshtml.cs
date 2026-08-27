using Common.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using ModelLayer.ViewModel;
using System.Net.Http.Headers;
using System.Security.Claims;


namespace EShope.Pages.Shop
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _setting;
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(
            IHttpClientFactory httpClientFactory,
            IOptions<SettingWeb> setting,
            ILogger<IndexModel> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _setting = setting.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            // تنظیم آدرس API از تنظیمات
            ApiBaseUrl = _setting.BaseAddress;
        }

        public List<ShopDto> Dtos { get; set; } = new List<ShopDto>();
        [TempData] public string? Message { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 12;
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        // آدرس API از تنظیمات
        public string ApiBaseUrl { get; private set; }
        public SettingWeb Setting => _setting;

        private async Task LoadShopAsync(int pageNumber, int pageSize, string searchTerm,string? sellerId, CancellationToken ct)
        {
            var client = _httpClientFactory.CreateClient(_setting.ClinetName);
            var request = new PagedRequest<ShopDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                StartIndex = (pageNumber - 1) * pageSize,
                Data = new ShopDto { ShopCode = searchTerm, ShopName = null, SellerId = sellerId }
            };

            try
            {
                var response = await client.PostAsJsonAsync("api/shop/pagination", request, ct);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<ShopDto>>>(ct);
                    if(result?.Data != null)
                    {
                        Dtos = result.Data;
                        PageNumber = result.PageNumber;
                        PageSize = result.PageSize;
                        TotalPages = result.TotalPages;
                        return;
                    }
                }
                ModelState.AddModelError(string.Empty, "خطا در دریافت اطلاعات");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت فروشگاه‌ها");
                ModelState.AddModelError(string.Empty, "خطا در اتصال به شبکه");
            }

            Dtos = new List<ShopDto>();
            TotalPages = 1;
        }
      
        public async Task<IActionResult> OnGetAsync(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("admin");
            var ownerFilter = isAdmin ? null : userId; // ادمین همه رو ببینه، فروشنده فقط خودش رو

            PageNumber = PageNumber <= 0 ? 1 : PageNumber;
            PageSize = PageSize <= 0 ? 12 : PageSize;    
            //if (PageNumber <= 0) PageNumber = 1;
            //if (PageSize <= 0) PageSize = 12;

            SearchTerm = string .IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim();

            await LoadShopAsync(PageNumber, PageSize, SearchTerm!,ownerFilter, ct);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Partial("_ShopList", this);
            }

            return Page();
        }

        // متد کمکی برای ساخت URL تصویر
        public string GetImageUrl(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || imagePath.Equals("null", StringComparison.OrdinalIgnoreCase))
                return "/images/default-avatar.jpg";

            var baseAddress = ApiBaseUrl?.TrimEnd('/') ?? "";

            var cleanPath = imagePath.Replace('\\', '/').Trim();
            if (!cleanPath.StartsWith("/"))
                cleanPath = "/" + cleanPath;

            if (string.IsNullOrEmpty(baseAddress))
                return cleanPath;
            try
            {
                var baseUri = new Uri(baseAddress);
                var finalUri = new Uri(baseUri + cleanPath);
                return finalUri.ToString();
            }
            catch
            {
                return cleanPath;
            }
        }

        public async Task<IActionResult> OnGetDetailsAsync(Guid id)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Unauthorized();

            if (id == Guid.Empty)
                return BadRequest(new { error = "شناسه نامعتبر" });

            try
            {
                var client = _httpClientFactory.CreateClient(_setting.ClinetName);

                // اگر نیاز به احراز هویت دارد، توکن را اضافه کن
                if (_setting.RequiresAuth)
                {
                    var token = _httpContextAccessor.HttpContext?.User
                        .FindFirst(_setting.TokenName)?.Value;

                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue(_setting.TokenType, token);
                    }
                }

                var response = await client.GetAsync($"api/shop/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var shop = await response.Content.ReadFromJsonAsync<ShopDto>();
                    return new JsonResult(shop);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new { error = "فروشگاه یافت نشد" });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Unauthorized();
                }
                else
                {
                    _logger.LogError("API returned {StatusCode} for shop {Id}", response.StatusCode, id);
                    return StatusCode(500, new { error = "خطای سرور" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت جزئیات فروشگاه {Id}", id);
                return StatusCode(500, new { error = "خطای سرور" });
            }
        }

        public async Task<IActionResult> OnPostLikeAsync(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_setting.ClinetName);
                var response = await client.PostAsync($"api/shop/{id}/like", null);

                if (response.IsSuccessStatusCode)
                {
                    // فرض می‌کنیم API بعد از لایک، اطلاعات به‌روز شده فروشگاه را برمی‌گرداند
                    var shop = await response.Content.ReadFromJsonAsync<ShopDto>();
                    return new JsonResult(new
                    {
                        newLikesCount = shop?.LikesCount ?? 0,
                        success = true
                    });
                }

                return new JsonResult(new
                {
                    newLikesCount = 0,
                    success = false,
                    error = "خطا در ثبت لایک"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ثبت لایک برای فروشگاه {Id}", id);
                return new JsonResult(new
                {
                    newLikesCount = 0,
                    success = false,
                    error = ex.Message
                });
            }
        }

        public async Task<IActionResult> OnPostDislikeAsync(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_setting.ClinetName);
                var response = await client.PostAsync($"api/shop/{id}/dislike", null);

                if (response.IsSuccessStatusCode)
                {
                    var shop = await response.Content.ReadFromJsonAsync<ShopDto>();
                    return new JsonResult(new
                    {
                        newDislikesCount = shop?.DislikesCount ?? 0,
                        success = true
                    });
                }

                return new JsonResult(new
                {
                    newDislikesCount = 0,
                    success = false,
                    error = "خطا در ثبت دیسلایک"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ثبت دیسلایک برای فروشگاه {Id}", id);
                return new JsonResult(new
                {
                    newDislikesCount = 0,
                    success = false,
                    error = ex.Message
                });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_setting.ClinetName);
                var response = await client.DeleteAsync($"api/shop/{id}");

                if (response.IsSuccessStatusCode)
                {
                    Message = "فروشگاه با موفقیت حذف شد.";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Message = "فروشگاه مورد نظر یافت نشد.";
                }
                else
                {
                    Message = "خطا در حذف فروشگاه. لطفاً دوباره تلاش کنید.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف فروشگاه {Id}", id);
                Message = "خطا در ارتباط با سرور.";
            }

            return RedirectToPage();
        }
    }
}