using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using System.Security.Claims;

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
        public ShopDto Dtos { get; set; } = new ShopDto();

        [TempData]
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {           
            try
            {
                var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
                var dto = await client.GetFromJsonAsync<ShopDto>($"api/Shop/{id}");
                if (dto == null)
                {
                    Message = "فروشنده مورد نظر یافت نشد.";
                    return RedirectToPage("./Index");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("admin");
                if (!isAdmin && dto.SellerId != userId) return Forbid();

                Dtos = dto;
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

            if (Dtos.Id == null || Dtos.Id == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "شناسه فروشنده معتبر نیست.");
                return Page();
            }
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

            var existing = await client.GetFromJsonAsync<ShopDto>($"api/shop/{Dtos.Id}");
            if (existing == null)
            {
                ModelState.AddModelError(string.Empty, "فروشگاه یافت نشد.");
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("admin");
            if (!isAdmin && existing.SellerId != userId)
            {
                return Forbid();
            }

            try
            {
                using var form = new MultipartFormDataContent();

                if (Dtos.Avatar != null && Dtos.Avatar.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await Dtos.Avatar.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    //ms.Position = 0;
                    var fileContent = new ByteArrayContent(bytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                        Dtos.Avatar.ContentType ?? "application/octet-stream");
                    form.Add(fileContent, "Avatar", Dtos.Avatar.FileName);
                }
                void AddString(string name, string? value)
                {
                    if (value != null)
                        form.Add(new StringContent(value), name);
                }

                AddString("Id", Dtos.Id.Value.ToString());
                AddString("ShopName", Dtos.ShopName);
                AddString("Description", Dtos.Description);
                AddString("ShopCode", Dtos.ShopCode);
                AddString("ImagePath", Dtos.ImagePath ?? ""); // در صورت نیاز

                // AddressDto فیلدها با نام AddressDto.PropertyName
                if (Dtos.AddressDto != null)
                {
                    //if (Seller.AddressDto.Id.HasValue)
                    //    AddString("AddressId", Seller.AddressDto.Id.Value.ToString());
                    AddString("AddressDto.Id", Dtos.AddressDto.Id?.ToString());
                    AddString("AddressDto.City", Dtos.AddressDto.City);
                    AddString("AddressDto.State", Dtos.AddressDto.State);
                    AddString("AddressDto.Tellphone", Dtos.AddressDto.Tellphone);
                    AddString("AddressDto.AdressDetail", Dtos.AddressDto.AdressDetail);
                }

                // ارسال PUT (بعضی سرورها با PUT+multipart مشکلی ندارند، اگر داشتین از POST مسیر جدا استفاده کنین)
                var requestUri = $"api/shop/{Dtos.Id}";
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
