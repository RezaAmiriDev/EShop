using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;

namespace EShope.Pages.Seller
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
        public SellerDto Seller { get; set; } = new SellerDto();


        [TempData]
        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
            try
            {
                var dto = await client.GetFromJsonAsync<SellerDto>($"api/seller/{id}");
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
                var resp = await client.PostAsJsonAsync($"api/seller/{Seller.Id}", Seller);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["Message"] = "ویرایش با موفقیت ذخیره شد.";
                    return RedirectToPage("./Index");
                }

                var errorContent = await resp.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"خطا از سرور: {(string.IsNullOrEmpty(errorContent) ? resp.StatusCode.ToString() : errorContent)}");
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
