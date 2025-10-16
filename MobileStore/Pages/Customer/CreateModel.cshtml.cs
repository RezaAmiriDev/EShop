using ClassLibrary;
using ClassLibrary.ViewModel;
using Common.Setting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public CusProDto Customer { get; set; } = new();
        //[BindProperty] 
        //public Address address { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid) return Page();
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
                var token = User.FindFirst(_settingWeb.TokenName);
                if (token == null)
                {
                    // توکن وجود ندارد -> logout
                    return RedirectToAction("SignOut", "Account");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_settingWeb.TokenType, token.Value);
                var response = await client.PostAsJsonAsync("api/Customer" , Customer);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        return RedirectToAction("SignOut", "Account");

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        return RedirectToPage("Index");

                    return RedirectToAction("ErrorPage", "Home");
                }
            }catch (Exception)
            {
                // در صورت نیاز لاگ کنید
                return RedirectToAction("ErrorPage", "Home");
            }
        }
    }
}
