using ClassLibrary.ViewModel;
using Common.Setting;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using ModelLayer.ViewModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EShope.Pages.Customer
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly SettingWeb _settingWeb;

        public EditModel(IHttpClientFactory httpFactory, IOptions<SettingWeb> options)
        {
            _httpFactory = httpFactory;
            _settingWeb = options.Value;
        }

        [BindProperty]
        public CusProDto Customer { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) return BadRequest();
            var client = _httpFactory.CreateClient(_settingWeb.ClinetName);
            var tokenClaim = User.FindFirst(_settingWeb.TokenName);
            if (tokenClaim == null) return RedirectToPage("/Account/SignOut");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_settingWeb.TokenType, tokenClaim.Value);

            var response = await client.GetAsync($"api/Customer/{id}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) return RedirectToPage("/Account/SignOut");
                return RedirectToAction("ErrorPage", "Home");

            }
            Customer = await response.Content.ReadFromJsonAsync<CusProDto>() ?? new CusProDto();
            // جلوگیری از NullReference در View
            Customer.addressDto ??= new AddressDto();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Customer.addressDto ??= new AddressDto();
                    TempData["Error"] = "لطفا اطلاعات فرم را به درستی تکمیل کنید";
                    return Page();
                }
                // اطمینان از مقداردهی addressDto قبل از ارسال به API
                Customer.addressDto ??= new AddressDto();

                var client = _httpFactory.CreateClient(_settingWeb.ClinetName);
                var token = User.FindFirst(_settingWeb.TokenName);
                if (token == null)
                {
                    TempData["Error"] = "دسترسی غیرمجاز";
                    return RedirectToPage("/Account/SignOut");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_settingWeb.TokenType, token.Value);
                var response = await client.PutAsJsonAsync($"api/Customer/{Customer.Id}", Customer, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "مشتری با موفقیت ویرایش شد";
                    return RedirectToPage("./Index");
                }

                // مدیریت خطاهای مختلف
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest:
                        try
                        {
                            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var err = await response.Content.ReadFromJsonAsync<ServiceResult>(opts, cancellationToken);
                            TempData["Error"] = err?.Message ?? "خطا در ویرایش اطلاعات";
                        }
                        catch
                        {
                            TempData["Error"] = "خطا در پردازش پاسخ سرور";
                        }
                        return Page();

                    case System.Net.HttpStatusCode.NotFound:
                        TempData["Error"] = "مشتری مورد نظر یافت نشد";
                        return Page();

                    case System.Net.HttpStatusCode.Unauthorized:
                        return RedirectToPage("/Account/SignOut");

                    default:
                        TempData["Error"] = "خطا در سیستم! مجددا تلاش کنید";
                        return Page();
                }

            }
            catch (OperationCanceledException)
            {
                TempData["Error"] = "درخواست لغو شد";
                return Page();
            }
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error in Edit OnPostAsync for customer {CustomerId}", Customer?.Id);
            //    TempData["Error"] = "خطا در ارتباط با سرور";
            //    return Page();
            //}
        }
    }
}
    