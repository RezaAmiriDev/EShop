using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using System.Net.Http.Headers;


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
            if (!ModelState.IsValid) return BadRequest();

            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

            try
            {
                using var form = new MultipartFormDataContent();
                void AddString(string name, string? value)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                        form.Add(new StringContent(value), name);
                }

                // ===== ShopDto fields =====
                AddString("ShopName", Seller.ShopName);
                AddString("Description", Seller.Description);
                AddString("ShopCode", Seller.ShopCode);
                AddString("ImagePath", Seller.ImagePath);

                // ===== AddressDto fields =====
                if (Seller.AddressDto != null)
                {
                    AddString("AddressDto.Id", Seller.AddressDto.Id?.ToString());
                    AddString("AddressDto.City", Seller.AddressDto.City);
                    AddString("AddressDto.State", Seller.AddressDto.State);
                    AddString("AddressDto.Tellphone", Seller.AddressDto.Tellphone);
                    AddString("AddressDto.AdressDetail", Seller.AddressDto.AdressDetail);
                }

                // ===== Avatar file =====
                if (Seller.Avatar != null && Seller.Avatar.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await Seller.Avatar.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    var fileContent = new ByteArrayContent(bytes);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(Seller.Avatar.ContentType ?? "application/octet-stream");
                    form.Add(fileContent, "Avatar", Seller.Avatar.FileName);
                }

                var response = await client.PostAsync("api/shop", form);
                if (response.IsSuccessStatusCode)
                    return RedirectToPage("./Index");

                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "خطا در ایجاد فروشگاه: " + error);
                return Page();

            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "خطا در ارتباط با سرور: " + ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "خطای غیرمنتظره: " + ex.Message);
                return Page();
            }
        }
    }
}
