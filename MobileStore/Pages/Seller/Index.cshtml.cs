using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.Interface;
using ModelLayer.ViewModel;
using ServiceLayer.Services;

namespace EShope.Pages.Seller
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _setting;

        public IndexModel(IHttpClientFactory httpClientFactory , SettingWeb settingWeb)
        {
            _httpClientFactory = httpClientFactory;
            _setting = settingWeb;
        }


        public IEnumerable<SellerDto> Sellers { get; set; } = new List<SellerDto>();

        [TempData]
        public string? Message {  get; set; }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient(_setting.ClinetName);
            try
            {
                var result = await client.GetFromJsonAsync<IEnumerable<SellerDto>>("api/seller");
                Sellers = result ?? new List<SellerDto>();
            }
            catch (Exception ex)
            {
                // لاگ‌گیری یا نمایش پیام مناسب
                Message = "خطا در دریافت اطلاعات: " + ex.Message;
                Sellers = new List<SellerDto>();
            }

        }

        // handler برای حذف (فرم حذف روی هر کارت این handler را فراخوانی می‌کند)
        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var client = _httpClientFactory.CreateClient(_setting.ClinetName);
            try
            {
                var resp = await client.DeleteAsync($"api/seller/{id}");
                if(resp.IsSuccessStatusCode)
                {
                    Message = "حذف با موفقیت انجام شد.";
                }
                else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) 
                {
                    Message = "فروشنده یافت نشد.";
                }
                else
                {
                    Message = "خطا در حذف. دوباره تلاش کنید.";
                }
            }
            catch
            {
                Message = "خطا در سرور هنگام حذف.";
            }

            return RedirectToPage(); // بازگشت و رفرش لیست
        }
    }
}
