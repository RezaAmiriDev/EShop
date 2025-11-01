using AutoMapper;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelLayer.ViewModel;
using ServiceLayer.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly ShopService _shopService;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ShopController(ShopService shopService , IWebHostEnvironment webHostEnvironment , IMapper mapper)
        {
            _shopService = shopService;
            _env = webHostEnvironment;
            _mapper = mapper;
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
        public async Task<IActionResult> Create([FromBody] ShopDto dto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                //if (file != null && file.Length > 0)
                //{
                //    var saved = await SaveAvatarAsync(file);
                //    dto.ImagePath = saved;
                //}
                if (!ModelState.IsValid) return BadRequest();
                // اگر Id نداشت، یکی بساز تا بتوانیم CreatedAtAction را برگردانیم
                if (dto.Id == null || dto.Id == Guid.Empty)
                {
                    dto.Id = Guid.NewGuid();
                }

                // اگر AddressDto وجود دارد و Id ندارد می‌توانیم مقدار دهیم (اختیاری)
                if (dto.AddressDto != null && (dto.AddressDto.Id == null || dto.AddressDto.Id == Guid.Empty))
                {
                    dto.AddressDto.Id = Guid.NewGuid();
                }

                // 3) ذخیره در دیتابیس
                var result = await _shopService.CreateAsync(dto);

                if (result.Status == ResponseStatus.Success)
                {
                    return CreatedAtAction(nameof(GetById), new { id = dto.Id }, null);
                }

                if (result.Status == ResponseStatus.BadRequest)
                {
                    return BadRequest(result.Message);
                }

                // سایر حالات => سرور
                return StatusCode(500, result.Message ?? "خطا در ذخیره فروشگاه");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Err in Creat controller", ex.Message);
                return StatusCode(500, "خطا در ذخیره فروشگاه");
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

        // helper
        private async Task<string> SaveAvatarAsync(IFormFile file)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) throw new InvalidDataException("File type not allowed.");

            var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "shops");
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploads, fileName);

            // محدودیت اندازه (مثال)
            if (file.Length > 5 * 1024 * 1024) throw new InvalidDataException("File too large.");

            await using(var stream = new FileStream(filePath , FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // مسیری که در DB ذخیره می‌کنیم (برای مرورگر)
            return $"/images/shops/{fileName}";
        }

        [HttpPut("{id:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] ShopDto dto, [FromForm] IFormFile? Avatar)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest();
            dto.Id = id; // اطمینان از اینکه Id در DTO مقداردهی شده است

            var existing = await _shopService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            try
            {

                // اگر فایل ارسال شده، ذخیره و مسیر در DTO قرار بده
                if (Avatar != null && Avatar.Length > 0)
                {
                    // حذف فایل قبلی (اختیاری)
                    if (!string.IsNullOrEmpty(existing.ImagePath))
                    {
                        var oldPhysical = Path.Combine(_env.WebRootPath ?? "wwwroot",
                            existing.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldPhysical))
                            System.IO.File.Delete(oldPhysical);
                    }

                    dto.ImagePath = await SaveAvatarAsync(Avatar);
                }
                else
                {
                    dto.ImagePath = string.IsNullOrWhiteSpace(dto.ImagePath) ? existing.ImagePath : dto.ImagePath;
                }

                // درخواست آپدیت به سرویس
                var result = await _shopService.UpdateAsync(dto);

                if (result.Status == ResponseStatus.Success)
                {
                    var updated = await _shopService.GetByIdAsync(id);
                    return Ok(updated);
                }
               
                if (result.Status == ResponseStatus.NotFound) return NotFound(result.Message);
                if (result.Status == ResponseStatus.BadRequest) return BadRequest(result.Message);

                return StatusCode(500, result.Message ?? "خطای سرور هنگام به‌روزرسانی");
            }
            catch (Exception ex)
            {
                // لاگ دقیق
                Console.WriteLine($"Update error: {ex}");
                return StatusCode(500 ,"خطا در بروزرسانی");
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file");

            var saved = await SaveAvatarAsync(file);
            return Ok(new { path = saved });
        }

    }
}
