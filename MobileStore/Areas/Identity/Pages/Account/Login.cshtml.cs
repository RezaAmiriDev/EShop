using DataLayer.Hellper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;


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
            ReturnUrl = returnUrl ?? Url.Content("~/Home/HomePage");
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
                HttpResponseMessage response;
                try
                {
                    // ارسال POST به API
                    response = await client.PostAsJsonAsync("api/Account/Login", Input);
                }
                catch (HttpRequestException httpEx)
                {
                    _logger.LogError(httpEx, "HttpRequestException calling API Login (Base: {Base})", client.BaseAddress);
                    ErrorMessage = "خطا در ارتباط با سرویس اعتبارسنجی: " + httpEx.Message;
                    return Page();
                }

               
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    try
                    {
                        var errorObj = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                        if(errorObj != null)
                        {
                            if(errorObj.TryGetValue("isLockedOut", out var locked) && locked is true)
                            {
                                ErrorMessage = errorObj.GetValueOrDefault("message")?.ToString() ?? "حساب کاربری شما قفل شده است.";
                                return Page();
                            }

                            // نمایش تعداد تلاش‌های باقی‌مانده
                            if (errorObj.TryGetValue("remainingAttempts", out var remaining))
                            {
                                ErrorMessage = errorObj.GetValueOrDefault("message")?.ToString()
                                               ?? "نام کاربری یا رمز عبور اشتباه است.";
                                return Page();
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled error in Login ");
                        ErrorMessage = $"خطای داخلی رخ داد.{ex.Message}";
                    }

                    ErrorMessage = string.IsNullOrWhiteSpace(content) ? response.ReasonPhrase : content;
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

                //var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var payload = JwtHelper.DecodeJwtPayload(loginResp.Token);
                // 2) ساخت ClaimsPrincipal و ساین-این محلی (Cookie auth)
                var claims = new List<Claim>
                {
                  new Claim(ClaimTypes.Name, loginResp.Username ?? Input.Username),
                  new Claim(tokenClaimName, loginResp.Token!)
                };

                if (payload.TryGetValue(ClaimTypes.Role, out var roleValue))
                {
                    if (roleValue.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var r in roleValue.EnumerateArray())
                            claims.Add(new Claim(ClaimTypes.Role, r.GetString()!));
                    }
                    else if (roleValue.ValueKind == JsonValueKind.String)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roleValue.GetString()!));
                    }
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                // اگر ReturnUrl معتبر و محلی است به آن برو، در غیر این صورت به /Customer هدایت کن
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }

                return RedirectToPage("/Home/HomePage");
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
