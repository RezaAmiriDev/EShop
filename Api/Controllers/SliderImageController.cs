using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.ViewModel;
using ServiceLayer.Services;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SliderImageController : ControllerBase
    {
        private readonly SliderService _sliderService;
        private readonly FileService _file;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public SliderImageController(SliderService sliderService, FileService fileService, IWebHostEnvironment webHost, IMapper mapper)
        {
            _sliderService = sliderService;
            _file = fileService;
            _env = webHost;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _sliderService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid Id) 
        {
            var sliders = await _sliderService.GetByIdAsync(Id);
            if(sliders == null) return NotFound();
            return Ok(sliders);
        } 

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SliderImageDto dto)
        {
           var result = await _sliderService.CreateAsync(dto, _env.WebRootPath);

            if (result.Status == DataLayer.ApiResult.ResponseStatus.Success)
                return Ok();

            return StatusCode(500);
         //   return CreatedAtAction(nameof(GetAll), new { id = slider.Id }, slider);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _sliderService.DeleteAsync(id, _env.WebRootPath);

            if(result.Status == DataLayer.ApiResult.ResponseStatus.Success)
               return NoContent();

            if(result.Status == DataLayer.ApiResult.ResponseStatus.NotFound)
                return NotFound();

            return StatusCode(500);
        }
    }
}
