using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using System.Text.Json;

namespace EShope.Pages.User
{
    [Authorize(Roles = "client")]
    public class UserIndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly SettingWeb _setting;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserIndexModel(IHttpClientFactory httpClient, SettingWeb setting)
        {
            _httpClient = httpClient;
            _setting = setting;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public List<UserDto> Users { get; set; } = new();
        public string? ErrorMessage { get; set; }

        private HttpClient CreateApiClient()
        {
            var client = _httpClient.CreateClient(_setting.ClinetName);
            var token = User.FindFirst(_setting.TokenName)?.Value;
            if(!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_setting.TokenType, token);
            }
            return client;
        }

        public async Task OnGetAsync()
        {
            try
            {

                var client = CreateApiClient();
                var response = await client.GetAsync("api/User");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Users = JsonSerializer.Deserialize<List<UserDto>>(content, _jsonOptions) ?? new();
                }
                else
                {
                    ErrorMessage = "خطا در دریافت لیست کاربران";
                    Users = new List<UserDto>();
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = $"خطا در ارتباط با سرور: {ex.Message}";
                Users = new List<UserDto>();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            try
            {
                var client = CreateApiClient();
                var response = await client.DeleteAsync($"api/User/{id}");
                if (response.IsSuccessStatusCode) return RedirectToPage();

                ModelState.AddModelError(string.Empty, "خطا در حذف کاربر");
                ErrorMessage = "خطا در حذف کاربر";
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
