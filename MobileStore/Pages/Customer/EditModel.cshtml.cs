using ClassLibrary.ViewModel;
using Common.Setting;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace EShope.Pages.Customer
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly SettingWeb _settingWeb;

        public EditModel(IHttpClientFactory httpFactory, IOptions<SettingWeb> options)
        {
            _httpFactory = httpFactory;
            _settingWeb = options.Value;
        }

        [BindProperty]
        public CusProDto Customer { get; set; } = new();
        
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == Guid.Empty) return BadRequest();
            var client = _httpFactory.CreateClient(_settingWeb.ClinetName);
            var token = User.FindFirst(_settingWeb.TokenName);
            if (token == null) return RedirectToAction("SignOut", "Account");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_settingWeb.TokenType, token.Value);

            var response = await client.GetAsync($"api/Customer/{id}");
            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) return RedirectToAction("SignOut", "Account");
                return RedirectToAction("ErrorPage", "Home");

            }
            Customer = await response.Content.ReadFromJsonAsync<CusProDto>() ?? new CusProDto();
            return Page();
        }
  
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();
            var client = _httpFactory.CreateClient(_settingWeb.ClinetName);
            var token = User.FindFirst(_settingWeb.TokenName);
            if (token == null) return RedirectToAction("SignOut", "Account");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_settingWeb.TokenType, token.Value);

            var response = await client.PutAsJsonAsync($"api/Customer/{Customer.Id}", Customer);
            if(response.IsSuccessStatusCode) return RedirectToPage("./Index");

            if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var err = await response.Content.ReadFromJsonAsync<ServiceResult>();
                ModelState.AddModelError(string.Empty, err?.Message ?? "خطا در ویرایش");
                return Page();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) 
            {
                return RedirectToPage("SignOut", "Account");
            }
           
            return RedirectToAction("ErrorPage", "Home");
        }
    }
}
