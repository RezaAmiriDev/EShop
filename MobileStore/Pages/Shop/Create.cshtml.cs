using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using System.Text.Json;

namespace EShope.Pages.Shop
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _settingWeb;
        public CreateModel(IHttpClientFactory httpClientFactory, SettingWeb settingWeb)
        {
            _httpClientFactory = httpClientFactory;
            _settingWeb = settingWeb;
        }

        [BindProperty]
        public ShopDto Seller { get; set; } = new ShopDto();

        public void OnGet()
        {
            // مطمئن شوید AddressDto مقداردهی شده است
            Seller.AddressDto ??= new AddressDto();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

            try
            {
                // 1) اگر فایل وجود دارد، اول آپلودش کن
                if (Seller.Avatar != null && Seller.Avatar.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await Seller.Avatar.CopyToAsync(ms);
                    ms.Position = 0;
                    using var content = new MultipartFormDataContent
                    {
                       {
                         new StreamContent(ms)
                         {
                           Headers = {
                             ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                             Seller.Avatar.ContentType ?? "application/octet-stream")
                           }
                         }, "file", Seller.Avatar.FileName
                       }
                    };

                    var uploadResp = await client.PostAsync("api/shop/upload", content);
                    if (!uploadResp.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError(string.Empty, "ارسال تصویر با خطا مواجه شد.");
                        return Page();
                    }
                    var uploadJson = await uploadResp.Content.ReadFromJsonAsync<JsonElement?>();
                    if (uploadJson.HasValue && uploadJson.Value.TryGetProperty("path", out var p))
                    {
                        Seller.ImagePath = p.GetString();
                    }
                       
                    Seller.Avatar = null;
                }

                // 2) حالا DTO را به صورت JSON بفرست
                var resp = await client.PostAsJsonAsync("api/shop", Seller);
                if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.Created)
                    return RedirectToPage("./Index");

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
