using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using System.Text.Json;

namespace EShope.Pages.Home
{
    public class HomePage : PageModel
    {
        private readonly IHttpClientFactory _client;
        private readonly SettingWeb _settingWeb;
        private readonly ILogger<IndexModel> _logger;

        public List<ProductCardDto> Products { get; set; } = new();
        public List<SliderImageDto> Sliders { get; set; } = new();

        public HomePage(IHttpClientFactory client, SettingWeb settingWeb, ILogger<IndexModel> logger)
        {
            _client = client;
            _settingWeb = settingWeb;
            _logger = logger;
        }

        public async Task OnGetAsync(int take = 12)
        {
            var client = _client.CreateClient(_settingWeb.ClinetName);
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                var resp = await client.GetAsync($"api/Home/Home?take={take}");
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Home Api returned {StatusCode}", resp.StatusCode);
                    SetFallbackData();
                    return;
                }
                //  (HomeDto)
                var home = await resp.Content.ReadFromJsonAsync<HomeDto>(option);
                if (home == null)
                {
                    _logger.LogWarning("Home API returned empty body");
                    SetFallbackData();
                    return;
                }

                Products = home.ProductItm ?? new List<ProductCardDto>();
                Sliders = home.SliderImg ?? new List<SliderImageDto>();

                var baseUrl = client.BaseAddress?.ToString().TrimEnd('/') ?? $"{Request.Scheme}://{Request.Host.Value}".TrimEnd('/');

                foreach (var p in Products)
                {
                    if (!string.IsNullOrWhiteSpace(p.ImagePath) && !p.ImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        p.ImagePath = $"{baseUrl}/{p.ImagePath.TrimStart('/')}";
                }

                foreach (var s in Sliders)
                {
                    if (!string.IsNullOrWhiteSpace(s.ImagePath) && !s.ImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        s.ImagePath = $"{baseUrl}/{s.ImagePath.TrimStart('/')}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home data");
                SetFallbackData();
            }

        }

        private void SetFallbackData()
        {
            Sliders = new List<SliderImageDto>
            {
                new SliderImageDto { Id = Guid.NewGuid(), ImagePath = "/images/hero1.jpg", Title = "Summer" },
                new SliderImageDto { Id = Guid.NewGuid(), ImagePath = "/images/placeholder.jpg", Title = "New" }
            };

            Products = new List<ProductCardDto>
            {
                new ProductCardDto { Id = Guid.NewGuid(), Name = "Sample 1", Brand = "adidas", ImagePath = "/images/p1.jpg", Price = 99, AverageRating = 4.5 },
                new ProductCardDto { Id = Guid.NewGuid(), Name = "Sample 2", Brand = "puma", ImagePath = "/images/p2.jpg", Price = 129, AverageRating = 4.0 },
                new ProductCardDto { Id = Guid.NewGuid(), Name = "Sample 1", Brand = "adidas", ImagePath = "/images/p3.jpg", Price = 99, AverageRating = 3.5 },
                new ProductCardDto { Id = Guid.NewGuid(), Name = "Sample 1", Brand = "adidas", ImagePath = "/images/p4.jpg", Price = 99, AverageRating = 2.0 },
            };
        }
    }
}

