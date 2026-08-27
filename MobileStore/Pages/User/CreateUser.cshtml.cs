using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;

namespace EShope.Pages.User
{
    [Authorize(Roles = "admin")]
    public class CreateUserModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _setting;

        public CreateUserModel(IHttpClientFactory httpClientFactory, SettingWeb setting)
        {
            _httpClientFactory = httpClientFactory;
            _setting = setting;
        }

        [BindProperty]
        public UserDto Dto { get; set; } = new UserDto();

        public IActionResult OnGet() => Page();

        private HttpClient CreateApiClient()
        {
            var client = _httpClientFactory.CreateClient(_setting.ClinetName);
            var token = User.FindFirst(_setting.TokenName)?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue(_setting.TokenType, token);
            }
            return client;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Dto.Password))
            {
                ModelState.AddModelError("dto.Password", "رمز عبور اجباری است");
                return Page();
            }

            if(!ModelState.IsValid) return Page();

            try
            {
                var client = CreateApiClient();
                var resp = await client.PostAsJsonAsync("api/User", Dto);
                if (resp.IsSuccessStatusCode) return RedirectToPage("UserIndex");

                var errorContent = await resp.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"خطا در ایجاد کاربر: {errorContent}");
                return Page();
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "خطا در ارتباط با سرور");
                return Page();
            }
        }

    }
}
