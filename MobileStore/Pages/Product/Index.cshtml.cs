using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelLayer.ViewModel;
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

        public List<ProductReadDto> readDto { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
                var resp = await client.GetAsync("api/Product");
                if (!resp.IsSuccessStatusCode) return;

                var json = await resp.Content.ReadAsStringAsync();
                readDto = JsonSerializer.Deserialize<List<ProductReadDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                var baseUri = client.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
                foreach (var p in readDto)
                {
                    if (!string.IsNullOrWhiteSpace(p.ImagePath) && !p.ImagePath.StartsWith("http", System.StringComparison.OrdinalIgnoreCase))
                        p.ImagePath = baseUri + "/" + p.ImagePath.TrimStart('/');
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                // در صورت خطا می‌توان یک پیام نشان داد یا لیست را خالی نگه داشت
                readDto = new List<ProductReadDto>();
            }
        }

        public async Task<IActionResult> OnGetSearchAsync(string name)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
                var resp = await client.GetAsync("api/Product");
                if (!resp.IsSuccessStatusCode)
                {
                    return BadRequest(new { message = "Failed to fetch products" });
                }

                var json = await resp.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<ProductReadDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                if (string.IsNullOrWhiteSpace(name))
                {
                    var term = name.Trim();
                    list = list.Where(p =>
                    (!string.IsNullOrWhiteSpace(p.Brand) && p.Brand.Contains(term, System.StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(p.Type.ToString()) && p.Type.ToString().Contains(term, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                var baseUri = client.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
                foreach(var p in  list)
                {
                    if(!string.IsNullOrWhiteSpace(p.ImagePath) && !p.ImagePath.StartsWith("http" , StringComparison.OrdinalIgnoreCase))
                        p.ImagePath = baseUri + "/" + p.ImagePath.TrimStart('/');
                }

                return new JsonResult(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search error");
                return StatusCode(500, new { message = "Server error" });
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
                    return new JsonResult(new { success = true });
                }
                else
                {
                    var text = await resp.Content.ReadAsStringAsync();
                    _logger.LogWarning("Delete failed: {Status} - {Text}", resp.StatusCode, text);
                    return new JsonResult(new { success = false, message = "Failed to delete product" });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete error");
                return new JsonResult(new { success = false, message = "Server error" });
            }
        }

    }
}
