using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EShope.Pages.User
{
    public class EditUserModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _setting;
        private readonly JsonSerializerOptions _jsonOptions;

        public EditUserModel(IHttpClientFactory httpClientFactory, SettingWeb setting)
        {
            _httpClientFactory = httpClientFactory;
            _setting = setting;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }


        [BindProperty]
        public UserDto Dto { get; set; } = new UserDto();

        public string? ErrorMessage { get; set; }

        private HttpClient CreateApiClient()
        {
            var client = _httpClientFactory.CreateClient(_setting.ClinetName);
            var token = User.FindFirst(_setting.TokenName)?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(_setting.TokenType, token);
            }
            return client;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                var client = CreateApiClient();
                var response = await client.GetAsync($"api/User/{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var content = await response.Content.ReadAsStringAsync();
                Dto = JsonSerializer.Deserialize<UserDto>(content, _jsonOptions) ?? new UserDto();
                return Page();
            }
            catch
            {
                ErrorMessage = "خطا در دریافت اطلاعات کاربر";
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (string.IsNullOrEmpty(Dto.Password)) Dto.Password = null;

            try
            {
                var client = CreateApiClient();
                var response = await client.PutAsJsonAsync($"api/User/{Dto.Id}", Dto);
                if (response.IsSuccessStatusCode) return RedirectToPage("UserInde");


                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"خطا در ویرایش کاربر: {errorContent}");
                return Page();
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "خطا در ارتباط با سرور");
                ErrorMessage = "خطا در ارتباط با سرور";
                return Page();
            }
        }
    }
}
