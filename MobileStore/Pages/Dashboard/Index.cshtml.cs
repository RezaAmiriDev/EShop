using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;

namespace EShope.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _Client;
        private readonly SettingWeb _seting;

        public IndexModel(SettingWeb seting, IHttpClientFactory client)
        {
            _Client = client;
            _seting = seting;
        }

        public DashboardSummaryDto Summary { get; set; } = new();
        public List<DailyTransactionDto> Daily { get; set; } = new();
        public List<CityOrderDto> Cities { get; set; } = new();
        public List<TopProductDto> TopProducts { get; set; } = new();

        public async Task OnGetAsync()
        {
            try {
                var client = _Client.CreateClient(_seting.ClinetName);
                var response = await client.GetAsync("api/dashboard/summary");
                if (!response.IsSuccessStatusCode)
                {
                    var text = await response.Content.ReadAsStringAsync();
                    ViewData["Error"] = $"API error {(int)response.StatusCode} {response.ReasonPhrase}: {text}";
                    return;
                }
                Summary = await response.Content.ReadFromJsonAsync<DashboardSummaryDto>() ?? new();
                //Summary = await client.GetFromJsonAsync<DashboardSummaryDto>("api/dashboard/summary") ?? new();

                Daily = await client.GetFromJsonAsync<List<DailyTransactionDto>>("api/dashboard/daily-transactions?days=15") ?? new();
                Cities = await client.GetFromJsonAsync<List<CityOrderDto>>("api/dashboard/top-cities?top=6") ?? new();
                TopProducts = await client.GetFromJsonAsync<List<TopProductDto>>("api/dashboard/top-products?top=6") ?? new();
            }
            catch (HttpRequestException ex)
            {
                ViewData["Error"] = "خطا در ارتباط با API: " + ex.Message;
            }

        }

    }
}
