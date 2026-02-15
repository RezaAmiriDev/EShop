using Azure;
using Common.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using ModelLayer.ViewModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

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

        public List<ShopDto> Sellers { get; set; } = new List<ShopDto>();
        [TempData] public string? Message { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 12;
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)] public string? SearchTerm { get; set; }

        // آدرس API از تنظیمات
        public string ApiBaseUrl { get; private set; }
        public SettingWeb Setting => _setting;

        public async Task OnGetAsync()
        {
            if (PageNumber <= 0) PageNumber = 1;
            if (PageSize <= 0) PageSize = 12;

            var client = _httpClientFactory.CreateClient(_setting.ClinetName);

            var request = new PagedRequest<ShopDto>
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                StartIndex = (PageNumber - 1) * PageSize,
                Data = new ShopDto
                {
                    ShopCode = string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim(),
                    ShopName = null
                }
            };

            try
            {
                // دریافت لیست فروشگاه‌ها از API
                var response = await client.PostAsJsonAsync("api/shop/pagination" , request);
           
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<ShopDto>>>();

                    if(result != null & result.Data != null)
                    {
                        Sellers = result.Data ?? new List<ShopDto>();
                        PageNumber = result.PageNumber > 0 ? result.PageNumber : PageNumber;
                        PageSize = result.PageSize > 0 ? result.PageSize : PageSize;
                        TotalPages = result.TotalPages > 0 ? result.TotalPages : 1;
                        //TotalRecords = result.TotalRecords;
                      //  TotalPages = (int)Math.Ceiling(result.TotalPages / (double)PageSize);
                      // _logger.LogInformation("Successfully loaded {Count} shop ", Sellers.Count);
                    }
                    else
                    {
                        Sellers = new List<ShopDto>();
                        _logger.LogWarning("API returned empty data for shops");
                    }
                }
                else
                {
                    Sellers = new List<ShopDto>();
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API returned {StatusCode} for shop list", response.StatusCode);
                    Message = "خطا در دریافت لیست فروشگاه‌ها";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت فروشگاه‌ها از API");
                Message = "خطا در اتصال به سرور";
                Sellers = new List<ShopDto>();
            }
        }

        // متد کمکی برای ساخت URL تصویر
        public string GetImageUrl(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)|| imagePath.Equals("null" , StringComparison.OrdinalIgnoreCase))
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