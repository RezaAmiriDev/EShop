using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using ModelLayer.ViewModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EShope.Pages.Product
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SettingWeb _settingWeb;


        public EditModel(IHttpClientFactory httpClientFactory, IOptions<SettingWeb> options)
        {
            _httpClientFactory = httpClientFactory;
            _settingWeb = options.Value;
        }



        [BindProperty]
        public ProductUpdateDto updateDto { get; set; } = new();
        public async Task<IActionResult> OnGet(Guid id)
        {
            var clienr = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
            var resp = await clienr.GetAsync($"api/Product/{id}");
            if (!resp.IsSuccessStatusCode) return RedirectToPage("/Products/Index");

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<ProductUpdateDto>(json , new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (dto == null) return RedirectToPage("/Products/Index");

            updateDto.Id = dto.Id; updateDto.Brand = dto.Brand; updateDto.Price = dto.Price; updateDto.Type = dto.Type;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(updateDto.Id.ToString()) , "Id");
            if(!string.IsNullOrWhiteSpace(updateDto.Brand)) content.Add(new StringContent(updateDto.Brand) , "Brand");
            content.Add(new StringContent(((int)updateDto.Type).ToString()), "Type");
            content.Add(new StringContent(updateDto.Price.ToString()), "Price");

            if(updateDto.ImageFile != null && updateDto.ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await updateDto.ImageFile.CopyToAsync(ms);
                ms.Position = 0;
                var fileContent = new ByteArrayContent(ms.ToArray());
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "ImageFile",
                    FileName = updateDto.ImageFile.FileName
                };
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(updateDto.ImageFile.ContentType ?? "application/octet-stream");
                content.Add(fileContent , "ImageFile" ,updateDto.ImageFile.FileName);
            }

            var resp = await client.PostAsync($"api/Product/{updateDto.Id}", content);
            if (resp.IsSuccessStatusCode) return RedirectToPage("/Products/Index");

            ModelState.AddModelError(string.Empty, "Failed to update product");
            return Page();

        }
    }
}
