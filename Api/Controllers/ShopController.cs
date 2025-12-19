using AutoMapper;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelLayer.ViewModel;
using ServiceLayer.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly ShopService _shopService;
        private readonly FileService _file;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ShopController(ShopService shopService , IWebHostEnvironment webHostEnvironment, IMapper mapper, FileService fileService)
        {
            _shopService = shopService;
            _env = webHostEnvironment;
            _mapper = mapper;
            _file = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GeyAll()
        {
            var items = await _shopService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var dto = await _shopService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ShopDto model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var image = model.Avatar;
            try
            {
                if(image != null && image.Length > 0)
                {
                    model.ImagePath = await _file.SaveFileAsync(image, _env.WebRootPath, "images/shops");
                }
                //// اگر Id نداشت، یکی بساز تا بتوانیم CreatedAtAction را برگردانیم
                //if (dto.Id == null || dto.Id == Guid.Empty) dto.Id = Guid.NewGuid();
                //if (dto.AddressDto != null && (dto.AddressDto.Id == null || dto.AddressDto.Id == Guid.Empty))
                //{
                //    dto.AddressDto.Id = Guid.NewGuid();
                //}

                // 3) ذخیره در دیتابیس
                var result = await _shopService.CreateAsync(model);

                if (result.Status != ResponseStatus.Success)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Err in Creat controller", ex.Message);
                return StatusCode(500, "خطا در ذخیره فروشگاه");
            }
        }

        //// helper
        //private async Task<string> SaveAvatarAsync(IFormFile file)
        //{
        //    var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        //    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        //    if (!allowed.Contains(ext)) throw new InvalidDataException("File type not allowed.");

        //    var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "shops");
        //    Directory.CreateDirectory(uploads);

        //    var fileName = $"{Guid.NewGuid()}{ext}";
        //    var filePath = Path.Combine(uploads, fileName);

        //    // محدودیت اندازه (مثال)
        //    if (file.Length > 5 * 1024 * 1024) throw new InvalidDataException("File too large.");

        //    await using(var stream = new FileStream(filePath , FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    // مسیری که در DB ذخیره می‌کنیم (برای مرورگر)
        //    return $"/images/shops/{fileName}";
        //}

        [HttpPut("{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] ShopDto dto)
        {
            var image = dto.Avatar;
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest();
            dto.Id = id; // اطمینان از اینکه Id در DTO مقداردهی شده است

            var existing = await _shopService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            try
            {
                if(image !=  null && image.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existing.ImagePath))
                    {
                        _file.DeleteFile(existing.ImagePath, _env.WebRootPath);
                    }

                    dto.ImagePath = await _file.SaveFileAsync(image, _env.WebRootPath, "images/shops");
                }
                else
                {
                    dto.ImagePath = existing.ImagePath;
                }

                var result = await _shopService.UpdateAsync(dto);
                if (result.Status != ResponseStatus.Success)
                {
                    return BadRequest();
                }

                if (result.Status == ResponseStatus.NotFound) return NotFound(result.Message);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // لاگ دقیق
                Console.WriteLine($"Update error: {ex}");
                return BadRequest(new ServiceResult(ResponseStatus.ServerError, null));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _shopService.DeleteAsync(id);
            if (result.Status == ResponseStatus.Success)
            {
                return NoContent();
            }
            if (result.Status == ResponseStatus.NotFound)
            {
                return NotFound(result.Message);
            }

            return StatusCode(500, result.Message);
        }


        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        //{
        //    if (file == null || file.Length == 0) return BadRequest("No file");

        //    var saved = await _shopService.SaveFileAsync(file , _env.WebRootPath, "images/shops");
        //    if (saved == null) return StatusCode(500, "Unable to save file");
        //    return Ok(new { path = saved });
        //}

    }
}
