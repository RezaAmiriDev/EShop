using WebFrameWork.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using DataLayer.Hellper;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// bind SettingWeb from configuration
var settingWebSection = builder.Configuration.GetSection("SettingWeb");
var settingWeb = settingWebSection.Get<SettingWeb>() ?? new SettingWeb();
builder.Services.Configure<SettingWeb>(settingWebSection);
builder.Services.AddSingleton(settingWeb);

// bind SettingWeb from appsettings.json
builder.Services.Configure<SettingWeb>(builder.Configuration.GetSection("SettingWeb"));
// اگر می‌خواهی SettingWeb را مستقیم (بدون IOptions) inject کنی، این خط را اضافه کن:
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<SettingWeb>>().Value);

// --- Services ---
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// ثبت DelegatingHandler
builder.Services.AddTransient<DataLayer.Hellper.AddBearerTokenHandler>();

//یک HttpClient به صورت Named Client ثبت شده.
//نامش _SettingWeb.ClinetName هست و BaseAddress اون _SettingWeb.BaseAddress.
//این یعنی بعداً هرجا IHttpClientFactory صدا بزنی می‌تونی همین کلاینت رو بسازی.
// register named HttpClient with BaseAddress from settings
builder.Services.AddHttpClient(settingWeb.ClinetName, client =>
{
    // ensure BaseAddress ends with slash to make relative requests like "api/Account/Login" safe
    var baseAddr = settingWeb.BaseAddress ?? "";
    if (!baseAddr.EndsWith("/")) baseAddr += "/";
    client.BaseAddress = new Uri(baseAddr);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// configure cookie authentication using SettingWeb values
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = settingWeb.Name ?? ".MyAppCookie";
        options.LoginPath = settingWeb.LoginPath ?? "/Identity/Account/Login";
        options.AccessDeniedPath = settingWeb.AccessDeniedPath ?? "/Account/Denied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        // options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // اگر خواستی
    });


builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
// فعال‌سازی دسترسی به فایل‌های استاتیک در پوشه uploads (اختیاری)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads"
});

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
// Map Razor Pages first or along with controllers:
app.MapRazorPages();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Customer}/{action=IndexModel}/{id?}");

// Map area controller route (اختیاری، فقط در صورت وجود controller در Area)
app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");

//// Root را به صفحهٔ لاگین هدایت می‌کنیم (تا کاربر اول لاگین ببیند)
//app.MapGet("/", () => Results.Redirect("/Identity/Account/Login"));

// keep root to Login so user first sees login page:
app.MapGet("/", () => Results.Redirect("/Identity/Account/Login"));

app.Run();
