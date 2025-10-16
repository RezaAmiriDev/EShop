public class SettingWeb
{
    public string ClinetName { get; set; } = "MyApi"; // typo kept to match your json
    public string BaseAddress { get; set; } = string.Empty;

    // اگر در کدهای PageModel/Controller از اینها استفاده می‌کنی:
    public string TokenName { get; set; } = "access_token"; // نام claim یا کوکی که توکن را نگه می‌داری
    public string TokenType { get; set; } = "Bearer";      // معمولاً "Bearer"
    public string LoginPath { get; set; } = "/Identity/Account/Login"; // مسیر لاگین
    public string LogoutPath { get; set; } = "/Account/Logout";
    public string Name { get; set; } = ".MyAppCookie";     // نام کوکی احراز هویت
    public string AccessDeniedPath { get; set; } = "/Account/Denied";
}
