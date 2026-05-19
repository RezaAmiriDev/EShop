using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using ModelLayer.ViewModel;
using System.Globalization;
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
        public ProductDto UpdateDto { get; set; } = new();
        public List<SelectListItem> ShopList { get; set; }

        public async Task<IActionResult> OnGet(Guid id, CancellationToken ct)
        {
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);
            // 1. دریافت فروشگاه 
            var shopResponse = await client.GetAsync($"/api/Shop/{id}", ct);
            if (shopResponse.IsSuccessStatusCode)
            {
                var shopRead = await shopResponse.Content.ReadAsStringAsync();
                var shopJson = JsonSerializer.Deserialize<List<ShopDto>>(shopRead, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                ShopList = shopJson?.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.ShopName }).ToList() ?? new();
            }
            else
            {
                ShopList = new List<SelectListItem>();
            }

            // 2. دریافت محصول 
            var resp = await client.GetAsync($"api/Product/{id}", ct);
            if (!resp.IsSuccessStatusCode) return RedirectToPage("/Products/Index");

            var json = await resp.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<ProductDto>(json , new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (dto == null) return RedirectToPage("/Product/Index");

            UpdateDto = dto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken ct)
        {
            if (!ModelState.IsValid) return Page();
            var client = _httpClientFactory.CreateClient(_settingWeb.ClinetName);

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(UpdateDto.Id.ToString()) , "Id");
            if(!string.IsNullOrWhiteSpace(UpdateDto.Brand)) content.Add(new StringContent(UpdateDto.Brand) , "Brand");
            if(!string.IsNullOrWhiteSpace(UpdateDto.Name)) content.Add(new StringContent(UpdateDto.Name), "Name");
            content.Add(new StringContent(((int)UpdateDto.Type).ToString()), "Type");
            content.Add(new StringContent(UpdateDto.Price?.ToString(CultureInfo.InvariantCulture) ?? "0"), "Price");
            content.Add(new StringContent(UpdateDto.ShopId.ToString()), "ShopId");
            content.Add(new StringContent(UpdateDto.ShortDescription ?? ""), "ShortDescription");
            
            if(UpdateDto.ImageFile != null && UpdateDto.ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await UpdateDto.ImageFile.CopyToAsync(ms);
                ms.Position = 0;
                var fileContent = new ByteArrayContent(ms.ToArray());
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "ImageFile",
                    FileName = UpdateDto.ImageFile.FileName
                };
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(UpdateDto.ImageFile.ContentType ?? "application/octet-stream");
                content.Add(fileContent , "ImageFile" ,UpdateDto.ImageFile.FileName);
            }

            var resp = await client.PutAsync($"api/Product/{UpdateDto.Id}", content, ct);
            if (resp.IsSuccessStatusCode) return RedirectToPage("/Product/Index");

            ModelState.AddModelError(string.Empty, "Failed to update product");
            return Page();

        }
    }
}
