using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace EShope.Areas.Identity.Pages.Account
{
    public class LoginModel(IHttpClientFactory httpClientFactory, ILogger<LoginModel> logger, IConfiguration configuration, SettingWeb settingWeb) : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ILogger<LoginModel> _logger = logger;
        private readonly SettingWeb _settingWeb = settingWeb;
        private readonly IConfiguration _configuration = configuration;

        [BindProperty]
        public LoginViewModel Input { get; set; } = new LoginViewModel();
        public string ErrorMessage { get; set; } = string.Empty;

        // دریافت ReturnUrl از query string در GET
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null!)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/Customer");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

                if(client.BaseAddress == null && !string.IsNullOrWhiteSpace(_settingWeb.BaseAddress))
                    client.BaseAddress = new Uri(_settingWeb.BaseAddress!.TrimEnd('/') + "/");

                // debugging: log base addresses
                _logger.LogInformation("HttpClient.BaseAddress = {BaseAddress}, SettingWeb.BaseAddress = {SettingBase}", client.BaseAddress, _settingWeb.BaseAddress);

                // try/catch around the HTTP call to log details
                HttpResponseMessage respon;
                try
                {
                    respon = await client.PostAsJsonAsync("api/Account/Login", Input);
                }
                catch (HttpRequestException httpEx)
                {
                    _logger.LogError(httpEx, "HttpRequestException calling API Login (Base: {Base})", client.BaseAddress);
                    ErrorMessage = "خطا در ارتباط با سرویس اعتبارسنجی: " + httpEx.Message;
                    if (httpEx.InnerException != null) _logger.LogError(httpEx.InnerException, "InnerException");
                    return Page();
                }
                catch (Exception exHttp)
                {
                    _logger.LogError(exHttp, "Unexpected exception when calling API Login");
                    ErrorMessage = "خطا در تماس با سرویس: " + exHttp.Message;
                    return Page();
                }

                // now we have a response object
                _logger.LogInformation("Login API returned {StatusCode}", respon.StatusCode);

                // ارسال POST به API
                var response = await client.PostAsJsonAsync("api/Account/Login", Input);

                if (!response.IsSuccessStatusCode)
                {
                    // گزینه: خواندن پیام خطا از بدنه
                    var err = await response.Content.ReadAsStringAsync();
                    ErrorMessage = "خطا در ورود: " + (string.IsNullOrWhiteSpace(err) ? response.ReasonPhrase : err);
                    return Page();
                }

                var loginResp = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResp == null || !loginResp.Success || string.IsNullOrEmpty(loginResp.Token))
                {
                    ErrorMessage = loginResp?.Message ?? "ورود موفقیت‌آمیز نبود.";
                    _logger.LogWarning("LoginResp null or invalid. Resp: {@Resp}", loginResp);
                    return Page();
                }

                // 1) ذخیره توکن در کوکی ایمن (برای استفاده در درخواست‌های بعدی)
                Response.Cookies.Append("X-Access-Token", loginResp.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(8)
                });

                // Ensure token claim name is not null/empty
                var tokenClaimName = string.IsNullOrWhiteSpace(_settingWeb.TokenName) ? "access_token" : _settingWeb.TokenName;
                // 2) ساخت ClaimsPrincipal و ساین-این محلی (Cookie auth)
                var claims = new List<Claim>
                {
                  new Claim(ClaimTypes.Name, loginResp.Username ?? Input.Username),
                  new Claim(tokenClaimName, loginResp.Token!)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                // اگر ReturnUrl معتبر و محلی است به آن برو، در غیر این صورت به /Customer هدایت کن
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }

                return RedirectToPage("/Customer/Index");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling auth API");
                ErrorMessage = "ارتباط با سرویس تأیید هویت برقرار نشد. لطفاً بعدا تلاش کنید.";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in Login");
                ErrorMessage = "خطای داخلی رخ داد.";
                return Page();
            }
        }
    }
}
