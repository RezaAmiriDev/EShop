using ClassLibrary;
using ClassLibrary.ViewModel;
using Common.Setting;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using System.Net.Http.Headers;

namespace EShope.Pages.Customer
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _settingWeb;
        //private readonly IFileService _fileService;

        public CreateModel(IHttpClientFactory httpClientFactory, SettingWeb settingWeb)
        {
            _httpClientFactory = httpClientFactory;
            _settingWeb = settingWeb;
            //_fileService = fileService;
        }

        [BindProperty]
        public CusProDto Customer { get; set; } = new CusProDto
        {
            addressDto = new AddressDto()
        };
        //[BindProperty] 
        //public Address address { get; set; }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Customer.addressDto ??= new AddressDto();
                    TempData["Error"] = "لطفا اطلاعات فرم را به درستی تکمیل کنید";
                    return Page();
                }
                // اطمینان از مقداردهی addressDto قبل از ارسال به API
                Customer.addressDto ??= new AddressDto();

                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
                var token = User.FindFirst(_settingWeb.TokenName);
                if (token == null)
                {
                    // توکن وجود ندارد -> logout
                    return RedirectToPage("/Account/SignOut");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_settingWeb.TokenType, token.Value);


                // لاگ داده‌های ارسالی
                Console.WriteLine($"Sending customer data: Name={Customer.Name}, NationalCode={Customer.NationalCode}");
                var response = await client.PostAsJsonAsync("api/Customer", Customer);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"API Response: {response.StatusCode} - {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "مشتری با موفقیت ایجاد شد";
                    return RedirectToPage("./Index");
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        return RedirectToPage("/Account/SignOut");

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        return RedirectToPage("Index");

                    // نمایش پیغام خطا از API
                    var errorResult = await response.Content.ReadFromJsonAsync<ServiceResult>();
                    TempData["Error"] = errorResult?.Message ?? "خطا در ذخیره اطلاعات";

                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in OnPostAsync: {ex.Message}");
                TempData["Error"] = "خطا در ارتباط با سرور";
                // در صورت نیاز لاگ کنید
                return RedirectToAction("ErrorPage", "Home");
            }
        }
    }
}
