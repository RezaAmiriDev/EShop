using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly HomeService _homeService;
        public HomeController(HomeService homeService) => _homeService = homeService;

        [HttpGet("Home")]
        public async Task<IActionResult> GetHome(int take = 12)
        {
            var dto = await _homeService.GetHomeAsync(take);

            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}".TrimEnd('/');

            // prefix تصاویر محصولات
            if (dto.ProductItm != null)
            {
                foreach (var p in dto.ProductItm!)
                {

                    if (!string.IsNullOrWhiteSpace(p.ImagePath) &&
                    !p.ImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        p.ImagePath = $"{baseUrl}/{p.ImagePath.TrimStart('/')}";
                    }
                }
            }

            // prefix تصاویر اسلایدر
            if (dto.SliderImg != null)
            {
                foreach (var s in dto.SliderImg!)
                {
                    if (!string.IsNullOrWhiteSpace(s.ImagePath) &&
                        !s.ImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        s.ImagePath = $"{baseUrl}/{s.ImagePath.TrimStart('/')}";
                    }
                }
            }
                

            return Ok(dto);
        }
    }
}
