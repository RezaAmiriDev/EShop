using Azure;
using Common.Pagination;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelLayer.ViewModel;
using System.Security.Claims;
using System.Text.Json;


namespace EShope.Pages.Product
{
    public class IndexModel : PageModel
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _settingWeb;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpClientFactory httpClientFactory, SettingWeb settingWeb, ILogger<IndexModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settingWeb = settingWeb;
            _logger = logger;
        }

        public List<ProductDto> proDto { get; set; } = new List<ProductDto>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 12;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public string? ApiBaseUrl { get; set; }

        public async Task OnGetAsync(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("admin");
            var sellerFilter = isAdmin ? null : userId;

            PageNumber = PageNumber <= 0 ? 1 : PageNumber;
            PageSize = PageSize <= 0 ? 1 : PageSize;
            SearchTerm = string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim();

            var request = new PagedRequest<ProductDto>
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                StartIndex = (PageNumber - 1) * PageSize,
                Data = new ProductDto
                {
                    Brand = SearchTerm,
                    ProductCode = SearchTerm,
                    SellerId = sellerFilter
                }
            };

            try
            {
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
                // ذخیره BaseAddress برای استفاده در View
                ApiBaseUrl = client.BaseAddress?.ToString().TrimEnd('/') ?? "";
               // ViewData["ApiBase"] = baseAddress;

                var response = await client.PostAsJsonAsync("api/Product/paged", request, ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API returned {StatusCode} for product list", response.StatusCode);
                    ModelState.AddModelError(string.Empty, $"خطا در دریافت اطلاعات از سرور (کد {response.StatusCode})");
                    proDto = new List<ProductDto>();
                    return;
                }
                   
                //var json = await response.Content.ReadAsStringAsync();
               // readDto = JsonSerializer.Deserialize<List<ProductDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                var result = await response.Content.ReadFromJsonAsync<PagedResponse<List<ProductDto>>>(ct);
                if(result?.Data == null /*result == null || request.Data == null*/)
                {
                    _logger.LogWarning("API returned empty data for products");
                    proDto = new List<ProductDto>();
                    TotalCount = 0;
                    TotalPages = 1;
                    return;
                }
                proDto = result.Data;
                TotalCount = result.TotalRecords;
                TotalPages = result.TotalPages;

                ViewData["Count"] = TotalCount;
                ViewData["ApiStatusCode"] = response.StatusCode;

                var baseUri = client.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
                foreach (var p in proDto)
                {
                    if (!string.IsNullOrWhiteSpace(p.ImagePath) && !p.ImagePath.StartsWith("http", System.StringComparison.OrdinalIgnoreCase))
                        p.ImagePath = baseUri + "/" + p.ImagePath.TrimStart('/');
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                proDto = new List<ProductDto>();
                ViewData["ApiError"] = ex.Message;

                ModelState.AddModelError("TRy", ex.Message);

            }
        }

        public async Task<IActionResult> OnGetSearchAsync(string term)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
                var resp = await client.GetAsync($"api/Product/search?term={Uri.EscapeDataString(term)}");
                if (!resp.IsSuccessStatusCode)
                {
                    return new JsonResult(Array.Empty<ProductDto>());
                }

                var json = await resp.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<ProductDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                var baseUri = client.BaseAddress?.ToString().TrimEnd('/') ?? "";
                foreach(var p in  list)
                {
                    if(!string.IsNullOrWhiteSpace(p.ImagePath) && !p.ImagePath.StartsWith("http"))
                        p.ImagePath = $"{baseUri}/{p.ImagePath.TrimStart('/')}";
                }

                return new JsonResult(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search error");
                return new JsonResult(Array.Empty<ProductDto>());
            }

        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
                var resp = await client.DeleteAsync($"api/Product/{id}");
                if (resp.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, message = "Product deleted successfully" });
                }
                else
                {
                    var errorContent = await resp.Content.ReadAsStringAsync();
                    _logger.LogWarning("Delete failed: {Status} - {Text}", resp.StatusCode, errorContent );
                    return new JsonResult(new { success = false, message = "Failed to delete product" });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete error");
                return new JsonResult(new { success = false, message = "Server error" });
            }
        }

        public async Task<IActionResult> OnGetDetailAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty) return BadRequest(new { error = "شناسه نامعتبر است" });
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

                var response = await client.GetAsync($"api/Product/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var dto = await response.Content.ReadFromJsonAsync<ProductDto>();
                    //var stringJson = await response.Content.ReadAsStringAsync();
                    return new JsonResult(dto, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("response from api to get detail faild !");
                    ModelState.AddModelError(string.Empty, "محصول مورد نظر یافت نشد");
                    return NotFound();
                }
                else
                {
                    _logger.LogError("API returned {StatusCode} for product {Id}", response.StatusCode, id);
                    return StatusCode(500, new { error = "خطای داخلی سرور" });
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "error loging detail");
                return StatusCode(500, new { error = "خطا در ارتباط با سرور" });
            }
        }
    }
}
