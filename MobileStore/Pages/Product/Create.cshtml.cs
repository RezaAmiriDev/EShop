using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using ModelLayer.ViewModel;
using System.Globalization;
using System.Net.Http.Headers;

namespace EShope.Pages.Product
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _settingWeb;


        public CreateModel(IHttpClientFactory httpClientFactory, IOptions<SettingWeb> options)
        {
            _httpClientFactory = httpClientFactory;
            _settingWeb = options.Value;
        }

        [BindProperty]
        public ProductDto dto { get; set; } = new ProductDto();
        public void OnGet() {}

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

            using var content = new MultipartFormDataContent();
        
            content.Add(new StringContent(dto.Brand ?? "") , "Brand");
            content.Add(new StringContent(((int)dto.Type).ToString()), "Type");
            content.Add(new StringContent(dto.Price?.ToString(CultureInfo.InvariantCulture) ?? "0"), "Price");

            if(dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.ImageFile.CopyToAsync(ms);
                var bytes = ms.ToArray();
                var fileContent = new ByteArrayContent(bytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType ?? "application/octet-stream");
                content.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
            }

            var resp = await client.PostAsync("api/Product", content);
            if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return RedirectToPage("/Product/Index");
            }

            var msg = await resp.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "خطا در ایجاد محصول: " + msg);
            return Page();
        }
    }
}
