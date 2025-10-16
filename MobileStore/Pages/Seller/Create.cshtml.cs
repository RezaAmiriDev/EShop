using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;

namespace EShope.Pages.Seller
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _settingWeb;
        public CreateModel(IHttpClientFactory httpClientFactory , SettingWeb settingWeb)
        {
            _httpClientFactory = httpClientFactory;
            _settingWeb = settingWeb;
        }

        [BindProperty]
        public SellerDto Seller { get; set; } = new SellerDto();

        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
            try
            {
                var resp = await client.PostAsJsonAsync("api/seller", Seller);
                if(resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return RedirectToPage("./Index");
                }

                var msg = await resp.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "خطا در ایجاد: " + msg);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "خطا در ارتباط با سرور: " + ex.Message);
                return Page();
            }

        }
    }
}
