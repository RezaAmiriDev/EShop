using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using System.Text.Json;

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

    public decimal TotalSales { get; set; }
    public List<OrderDto> CustomerSales { get; set; } = new();
    public List<OrderDto> ProductSales { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

            // --- اضافه کن: BaseAddress را برای view بفرست (trim '/' ته URL)
            var apiBase = client.BaseAddress?.ToString().TrimEnd('/') ?? "";
            ViewData["ApiBase"] = apiBase;
            var response = await client.GetAsync("api/Order/total");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var totalResp = JsonSerializer.Deserialize<ServiceResultByData<decimal>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (totalResp?.Status == ResponseStatus.Success)
                {
                    TotalSales = totalResp.Data;
                }
                else
                {
                    ErrorMessage = totalResp?.Message ?? "خطا در دریافت اطلاعات";
                }
            }
            else
            {
                ErrorMessage = $"خطای سرور: {response.StatusCode}";
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API Error {StatusCode}: {Content}", response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "خطا در اتصال به سرور";
            _logger.LogError(ex, "Error in OnGetAsync");
        }
    }

    public async Task<IActionResult> OnGetCustomerAsync(Guid customerId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
            var response = await client.GetAsync($"api/Order/customer/{customerId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var resp = JsonSerializer.Deserialize<ServiceResultByData<List<OrderDto>>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resp?.Status == ResponseStatus.Success)
                {
                    CustomerSales = resp.Data ?? new();
                    return Page();
                }
                else
                {
                    ErrorMessage = resp?.Message ?? "خطا در دریافت اطلاعات مشتری";
                    return Page();
                }
            }
            else
            {
                ErrorMessage = $"002 خطای سرور: {response.StatusCode}";
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Customer API Error {StatusCode}: {Content}", response.StatusCode, errorContent);
                return Page();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "خطا در اتصال به سرور";
            _logger.LogError(ex, "Error in OnGetCustomerAsync");
            return Page();
        }
    }

    public async Task<IActionResult> OnGetProductAsync(Guid productId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
            var response =   await client.GetAsync($"api/order/product/{productId}");
            if (response.IsSuccessStatusCode) 
            {
                var json = await response.Content.ReadAsStringAsync();
                var resp = JsonSerializer.Deserialize<ServiceResultByData<List<OrderDto>>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resp?.Status == ResponseStatus.Success)
                {
                    ProductSales = resp.Data ?? new();
                    return Page();
                }

                else
                {
                    ErrorMessage = resp?.Message ?? "خطا در دریافت اطلاعات محصول";
                    return Page();
                }
            }
            else
            {
                ErrorMessage = $"خطای سرور:  : {response.StatusCode}";
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Product API Error {StatusCode} : {Content}", response.StatusCode, errorContent);
                return Page();
            }     
        }
        catch (Exception ex)
        {
            ErrorMessage = "خطا در اتصال به سرور";
            _logger.LogError(ex, "Error in OnGetProductAsync");
            return Page();
        }
    }

    //public async Task<IActionResult> GetRecentPayments(int page = 1, int pageSize = 12)
    //{
    //    try
    //    {
    //        var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
    //        var resp = await client.GetAsync($"api/order/payments?page={page}&pageSize={pageSize}");

    //        var content = await resp.Content.ReadAsStringAsync();
    //        return Content(content, "application/json");
    //    }catch(Exception ex)
    //    {
    //        _logger.LogError(ex, "Error proxying payments");
    //        var error = new { status = "Error", message = ex.Message };
    //        return new JsonResult(error) { StatusCode = 500 };
    //    }
    //}

}
