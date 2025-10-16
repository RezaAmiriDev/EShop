using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;

namespace EShope.Pages.Sale
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _settingWeb;
        public string? ErrorMessage { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, SettingWeb settingWeb)
        {
            _httpClientFactory = httpClientFactory;
            _settingWeb = settingWeb;
        }

        public decimal TotalSales { get; set; }
        public List<SaleDto> CustomerSales { get; set; } = new();
        public List<SaleDto> ProductSales { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

                var totalResp = await client.GetFromJsonAsync<ServiceResultByData<decimal>>("api/sale/total");
                if (totalResp != null && totalResp.Status == ResponseStatus.Success)
                {
                    TotalSales = totalResp.Data;
                }
                else
                {
                    TotalSales = 0m;
                    if (totalResp != null)
                        ErrorMessage = totalResp.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task GetCustomerSalesAsync(Guid customerId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
                var resp = await client.GetFromJsonAsync<ServiceResultByData<List<SaleDto>>>($"api/sale/customer/{customerId}");
                if (resp != null && resp.Status == ResponseStatus.Success)
                {
                    CustomerSales = resp.Data ?? new List<SaleDto>();
                    ErrorMessage = null;
                }
                else
                {
                    CustomerSales = new List<SaleDto>();
                    ErrorMessage = resp?.Message ?? "خطای نامشخص در دریافت فروش‌ها";
                }
            }
            catch (Exception ex)
            {
                CustomerSales = new List<SaleDto>();
                ErrorMessage = ex.Message;
            }
        }

        public async Task GetProductSalesAsync(Guid productId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

                var resp = await client.GetFromJsonAsync<ServiceResultByData<List<SaleDto>>>($"api/sale/product/{productId}");
                if (resp != null && resp.Status == ResponseStatus.Success)
                {
                    ProductSales = resp.Data ?? new List<SaleDto>();
                    ErrorMessage = null;
                }
                else
                {
                    ProductSales = new List<SaleDto>();
                    ErrorMessage = resp?.Message ?? "خطای نامشخص در دریافت فروش‌ها";
                }

            }
            catch (Exception ex)
            {
                ProductSales = new List<SaleDto>();
                ErrorMessage = ex.Message;
            }
        }
    }
}
