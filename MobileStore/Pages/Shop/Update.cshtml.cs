using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;

namespace EShope.Pages.Shop
{
    public class UpdateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _settingWeb;
        public UpdateModel(IHttpClientFactory httpClientFactory, SettingWeb settingWeb)
        {
            _httpClientFactory = httpClientFactory;
            _settingWeb = settingWeb;
        }

        [BindProperty]
        public ShopDto Seller { get; set; } = new ShopDto();

        [TempData]
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
            try
            {

                var dto = await client.GetFromJsonAsync<ShopDto>($"api/Shop/{id}");
                if (dto == null)
                {
                    Message = "فروشنده مورد نظر یافت نشد.";
                    return RedirectToPage("./Index");
                }

                Seller = dto;
                return Page();
            }
            catch (HttpRequestException ex)
            {
                // شبکه یا دسترسی به API مشکل دارد
                Message = "خطا در ارتباط با سرور: " + ex.Message;
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                Message = "خطا: " + ex.Message;
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (Seller.Id == null || Seller.Id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "شناسه فروشنده معتبر نیست.");
                return Page();
            }
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
            try
            {
                using var form = new MultipartFormDataContent();

                if (Seller.Avatar != null && Seller.Avatar.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await Seller.Avatar.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    //ms.Position = 0;
                    var fileContent = new ByteArrayContent(bytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                        Seller.Avatar.ContentType ?? "application/octet-stream");
                    form.Add(fileContent, "Avatar", Seller.Avatar.FileName);
                }
                void AddString(string name, string? value)
                {
                    if (value != null)
                        form.Add(new StringContent(value), name);
                }

                AddString("Id", Seller.Id.Value.ToString());
                AddString("ShopName", Seller.ShopName);
                AddString("Description", Seller.Description);
                AddString("ShopCode", Seller.ShopCode);
                AddString("ImagePath", Seller.ImagePath ?? ""); // در صورت نیاز

                // AddressDto فیلدها با نام AddressDto.PropertyName
                if (Seller.AddressDto != null)
                {
                    //if (Seller.AddressDto.Id.HasValue)
                    //    AddString("AddressId", Seller.AddressDto.Id.Value.ToString());
                    AddString("AddressDto.Id", Seller.AddressDto.Id?.ToString());
                    AddString("AddressDto.City", Seller.AddressDto.City);
                    AddString("AddressDto.State", Seller.AddressDto.State);
                    AddString("AddressDto.Tellphone", Seller.AddressDto.Tellphone);
                    AddString("AddressDto.AdressDetail", Seller.AddressDto.AdressDetail);
                }

                // ارسال PUT (بعضی سرورها با PUT+multipart مشکلی ندارند، اگر داشتین از POST مسیر جدا استفاده کنین)
                var requestUri = $"api/shop/{Seller.Id}";
                var request = new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = form };
                var resp = await client.SendAsync(request);


                if (resp.IsSuccessStatusCode)
                {
                    TempData["Message"] = "ویرایش با موفقیت ذخیره شد.";
                    return RedirectToPage("./Index");
                }

                var errorContent = await resp.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"خطا از سرور: {errorContent}");
                return Page();
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "خطای ارتباط با سرور: " + ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "خطا: " + ex.Message);
                return Page();
            }

        }
    }
}
