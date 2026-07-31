using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModelLayer.ViewModel;
using NuGet.Packaging.Signing;

namespace EShope.Pages.SliderImage
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;
        private readonly SettingWeb _stting;

        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger, SettingWeb stting)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _stting = stting;
        }

        [BindProperty]
        public List<SliderImageDto> Sliders { get; set; } = new();

        [BindProperty]
        public SliderImageDto NewSlider { get; set; } = new();

        [TempData]
        public string Message { get; set; }

        public async Task OnGetAsync(CancellationToken ct)
        {
            var client = _httpClientFactory.CreateClient(_stting.ClinetName);
            var response = await client.GetAsync("api/SliderImage", ct);
            if (response.IsSuccessStatusCode)
            {
                Sliders = await response.Content.ReadFromJsonAsync<List<SliderImageDto>>(ct);
            }
            else
            {
                var erorrContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("fiald to get sliders {StatusCode}", response.StatusCode);
                ModelState.AddModelError("err !", string.Empty);
                return;
            }
        }

        public async Task<IActionResult> OnPostCreateAsync(CancellationToken ct)
        {
            if(NewSlider?.IFormFile == null)
            {
                ModelState.AddModelError(string.Empty, "model stat err");
                await OnGetAsync(ct);
                return Page();
            }

            var client = _httpClientFactory.CreateClient(_stting.ClinetName);
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(NewSlider.Title ?? ""), "Title");
            content.Add(new StringContent(NewSlider.Description ?? ""), "Description");

            using var ms = new MemoryStream();
            await NewSlider.IFormFile.CopyToAsync(ms, ct);
            ms.Position = 0;
            var fileContent = new ByteArrayContent(ms.ToArray());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(NewSlider.IFormFile.ContentType ?? "application/octet-stream");
            content.Add(fileContent, "IFormFile", NewSlider.IFormFile.FileName);

            var response = await client.PostAsync("api/SliderImage", content, ct);
            if (response.IsSuccessStatusCode)
            {
                Message = "Success THUTUTUTههههههه";
                return RedirectToPage();
            }
            else
            {
                var erorr = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Create slider failed: {StatusCode} - {Error}", response.StatusCode, erorr);
                ModelState.AddModelError(string.Empty, "Create faild !");
                return Page();
            }
        }
  
        public async Task<IActionResult> OnPostDeleteAsync(Guid id , CancellationToken ct)
        {
            var client = _httpClientFactory.CreateClient(_stting.ClinetName);
            var response = await client.DeleteAsync($"api/SliderImage/{id}", ct);
            if(response.IsSuccessStatusCode)
            {
                Message = "اسلایدر با موفقیت حذف شد.";
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ModelState.AddModelError(string.Empty, "اسلایدر مورد نظر یافت نشد.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "خطا در حذف اسلایدر.");
            }

            return RedirectToPage(); // بارگذاری مجدد صفحه و اجرای OnGetAsync
        }
        public string GetImageUrl(string imagePath)
        {
            if(string.IsNullOrEmpty(imagePath)) return "/images/default-slider.jpg";
            var baseUri = _stting.BaseAddress?.TrimEnd('/');
            return $"{baseUri}{imagePath}";
        }
    }
}
