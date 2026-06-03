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
            if (!response.IsSuccessStatusCode)
            {
                var erorrContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("fiald to get sliders {StatusCode}", response.StatusCode);
                ModelState.AddModelError("??? ?? ????? ?????? ?? !", string.Empty);
                return;
            }
        }

        public async Task<IActionResult> OnPostCreatAsync(CancellationToken ct)
        {
            if(NewSlider?.Slider == null)
            {
                ModelState.AddModelError(string.Empty, "????? ?? ????? ?????? ????");
                await OnGetAsync(ct);
                return Page();
            }

            var client = _httpClientFactory.CreateClient(_stting.ClinetName);
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(NewSlider.Title ?? ""), "Title");
            content.Add(new StringContent(NewSlider.Description ?? ""), "Description");

            using var ms = new MemoryStream();
            await NewSlider.Slider.CopyToAsync(ms, ct);
            ms.Position = 0;
            var fileContent = new ByteArrayContent(ms.ToArray());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(NewSlider.Slider.ContentType ?? "application/octet-stream");
            content.Add(fileContent, "ImageFile", NewSlider.Slider.FileName);

            var response = await client.PostAsync("api/SliderImage", content, ct);
            if (response.IsSuccessStatusCode)
            {
                Message = "??????? ?? ?????? ????? ??";
                return RedirectToPage();
            }
            else
            {
                var erorr = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Create slider failed: {StatusCode} - {Error}", response.StatusCode, erorr);
                ModelState.AddModelError(string.Empty, "??? ?? ????? ???????");
                return Page();
            }
        }
    }
}
