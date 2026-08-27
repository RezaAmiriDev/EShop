using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using ModelLayer.Models;
using ModelLayer.ViewModel;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EShope.Pages.Product
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _settingWeb;
        private readonly ILogger<CreateModel> _logger; 

        public CreateModel(IHttpClientFactory httpClientFactory, IOptions<SettingWeb> options, ILogger<CreateModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settingWeb = options.Value;
            _logger = logger;
        }

        [BindProperty]
        public ProductDto dto { get; set; } = new ProductDto();
        public List<SelectListItem> ShopList {  get; set; }

        public async Task OnGet(CancellationToken ct = default) 
        {
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("admin");

            try
            {
                var url = isAdmin ? "api/shop" : $"api/shop?sellerId={Uri.EscapeDataString(userId!)}";
                var response = await client.GetAsync("api/shop", ct);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(ct);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var shops = System.Text.Json.JsonSerializer.Deserialize<List<ShopDto>>(content,
                            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        ShopList = shops?.Select(s => new SelectListItem
                        {
                            Value = s.Id.ToString(),
                            Text = s.ShopName
                        }).ToList() ?? new List<SelectListItem>();
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to load shops. Status: {StatusCode}", response.StatusCode);
                    ModelState.AddModelError(string.Empty, "erorr in create Product page");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading shops for product creation");
                ModelState.AddModelError(string.Empty, "erorr in create Product page");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("admin");

            if (!isAdmin)
            {
                var shopResp = await client.GetAsync($"api/shop/{dto.ShopId}");
                if (!shopResp.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "فروشگاه انتخاب‌شده معتبر نیست.");
                    return Page();
                }
                var shop = await shopResp.Content.ReadFromJsonAsync<ShopDto>();
                if(shop == null || shop.SellerId != userId)
                {
                    return Forbid();
                }
            }

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(dto.Name ?? ""), "Name");
            content.Add(new StringContent(dto.Brand ?? ""), "Brand");
            content.Add(new StringContent(((int)dto.Type).ToString()), "Type");
            content.Add(new StringContent(dto.Price?.ToString(CultureInfo.InvariantCulture) ?? "0"), "Price");
            content.Add(new StringContent(dto.ShopId.ToString()), "ShopId");
            content.Add(new StringContent(dto.ShortDescription ?? ""), "ShortDescription");

            if(dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.ImageFile.CopyToAsync(ms);
                var bytes = ms.ToArray();
                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType ?? "application/octet-stream");
                content.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
            }

            var resp = await client.PostAsync("api/Product", content);
            if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return RedirectToPage("/Product/Index");
            }

            var msg = await resp.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "خطا در ایجاد محصول: " + msg);
            return Page();
        }
    }
}
