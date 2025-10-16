using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;


namespace EShope.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RegisterModel> _logger;
        private readonly SettingWeb _setting;

        public RegisterModel(IHttpClientFactory httpClientFactory, ILogger<RegisterModel> logger, SettingWeb setting)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _setting = setting;
        }

        [BindProperty]
        public InputModel input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "نام کاربری لازم است")]
            [Display(Name = "نام کاربری")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "ایمیل لازم است")]
            [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
            [Display(Name = "ایمیل")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "رمز عبور لازم است")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "طول رمز باید حداقل 6 نویسه باشد")]
            [DataType(DataType.Password)]
            [Display(Name = "رمز عبور")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "تأیید رمز لازم است")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "رمز و تکرار آن یکسان نیستند")]
            [Display(Name = "تکرار رمز عبور")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            try
            {
                var client = _httpClientFactory.CreateClient(_setting.ClinetName);

                // درخواست POST به API
                var response = await client.PostAsJsonAsync("api/Account/Register", new
                {
                    Username = input.Username,
                    Email = input.Email,
                    Password = input.Password,
                    ConfirmPassword = input.ConfirmPassword,
                });

                if (response.IsSuccessStatusCode)
                {
                    // ثبت موفق → هدایت به صفحه لاگین (یا هر صفحه‌ی دیگری)
                    TempData["SuccessMessage"] = "ثبت‌نام با موفقیت انجام شد. لطفاً وارد شوید.";
                    // اگر از Identity area login استفاده می‌کنید:
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                // اگر ناموفق بود، تلاش می‌کنیم خطاهای ساختاری را بخوانیم و به ModelState اضافه کنیم
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Register API returned {Status}: {Content}", (int)response.StatusCode, content);

                try
                {
                    // تلاش برای خواندن به عنوان Dictionary<string, string[]> (ساختار معمول خطاهای ولیدیشن)
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string[]>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if(dict != null && dict.Count > 0)
                    {
                        foreach(var kv in dict)
                        {
                            foreach(var msg in kv.Value)
                            {
                                ModelState.AddModelError(string.Empty, msg);
                            }
                            return Page();
                        }
                    }
                }
                catch { }
                // 2) تلاش برای خواندن به عنوان ProblemDetails
                try
                {
                    var pd = JsonSerializer.Deserialize<Microsoft.AspNetCore.Mvc.ProblemDetails>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (pd != null)
                    {
                        if (!string.IsNullOrWhiteSpace(pd.Detail))
                            ModelState.AddModelError(string.Empty, pd.Detail);
                        else if (!string.IsNullOrWhiteSpace(pd.Title))
                            ModelState.AddModelError(string.Empty, pd.Title);
                        return Page();
                    }
                }
                catch { /* ignore */ }

                // 3) اگر API آرایه‌ای از خطاهای Identity بازگرداند (مثلاً [{Code:..., Description:...}, ...])
                try
                {
                    var arr = JsonSerializer.Deserialize<IEnumerable<IdentityErrorDto>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (arr != null && arr.Any())
                    {
                        foreach (var e in arr)
                        {
                            ModelState.AddModelError(string.Empty, e.Description ?? $"{e.Code}");
                        }
                        return Page();
                    }
                }
                catch { /* ignore */ }

                // 4) به‌عنوان fallback متن خام را نمایش بده
                if (!string.IsNullOrWhiteSpace(content))
                {
                    ModelState.AddModelError(string.Empty, content);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"درخواست ناموفق شد. کد: {(int)response.StatusCode} ({response.ReasonPhrase})");
                }

                return Page();
            }
            catch (HttpRequestException hex)
            {
                _logger.LogError(hex, "Error connecting to API at {Base}", _setting.BaseAddress);
                ModelState.AddModelError(string.Empty, "خطا در ارتباط با سرویس ثبت‌نام. لطفاً بعداً تلاش کنید.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in registration.");
                ModelState.AddModelError(string.Empty, "خطای داخلی رخ داد.");
                return Page();
            }
        }

        // برای پارس کردن آرایه خطاهای Identity از API (اختیاری)
        private record IdentityErrorDto(string Code, string Description);
    }
}
       