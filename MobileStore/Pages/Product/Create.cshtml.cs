using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using ModelLayer.ViewModel;
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
        public ProductCreateDto createDto { get; set; }
        public void OnGet() {}

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

            using var content = new MultipartFormDataContent();
            if(!string.IsNullOrWhiteSpace(createDto.Brand)) 
                content.Add(new StringContent(createDto.Brand) , "Brand");
            content.Add(new StringContent(((int)createDto.Type).ToString()), "Type");
            content.Add(new StringContent(createDto.Price.ToString()), "Price");

            if(createDto.ImageFile != null && createDto.ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await createDto.ImageFile.CopyToAsync(ms);
                ms.Position = 0;
                var fileContent = new ByteArrayContent(ms.ToArray());
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "ImageFile",
                    FileName = createDto.ImageFile.FileName
                };
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(createDto.ImageFile.ContentType ?? "application/octet-stream");
                content.Add(fileContent, "ImageFile", createDto.ImageFile.FileName);
            }
            var resp = await client.PostAsync("api/Product", content);
            if (resp.IsSuccessStatusCode) return RedirectToPage("/Products/Index");

            ModelState.AddModelError(string.Empty, "Failed to create product");
            return Page();
        }
    }
}
