using ClassLibrary.Services;
using Common.Pagination;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.ViewModel;
using ServiceLayer.Services;


namespace MobileStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductRepo> _logger;
        private readonly FileService _file;
        public ProductController(ProductService productService, IWebHostEnvironment webHostEnvironment, ILogger<ProductRepo> logger , FileService fileService)
        {
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _file = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid id)
        {
            try
            {
                var products = await _productService.GetAllAsync();
                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest(new ServiceResult(ResponseStatus.ServerError, null));
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetByPagination(PagedRequest<ProductDto> paged)
        {
            try
            {
                if(paged == null)
                {
                    return BadRequest(new ServiceResult(ResponseStatus.BadRequest, "Invalid request payload"));
                }

                var pageNumber = paged.PageNumber <= 0 ? 1 : paged.PageNumber;
                var pageSize = paged.PageSize <= 0 ? 12 : paged.PageSize;

                paged.PageNumber = pageNumber;
                paged.PageSize = pageSize;
                paged.StartIndex = (pageNumber - 1) * pageSize;

                var result = await _productService.GetByPgination(paged);

             //   var outPaged = new PagedResult<List<ProductDto>>(servicePaged);
                var response = new PagedResponse<List<ProductDto>>(pageNumber, pageSize, result.TotalRecords, result.Data);
                
                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm]ProductDto dto , CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {
                if(dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    dto.ImagePath = await _file.SaveFileAsync(dto.ImageFile , _webHostEnvironment.WebRootPath , "images/products");
                }

                var result = await _productService.CreateAsync(dto , ct);
                if (result.Status != ResponseStatus.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest(new ServiceResult(ResponseStatus.ServerError, null));
            }
          }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,[FromForm] ProductDto dto ,CancellationToken token = default)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            if (dto == null) return BadRequest();
            dto.Id = id;

            var existing = await _productService.GetByIdAsync(id, token);
            if (existing == null) return NotFound("محصول یافت نشد.");
            try
            {
                if(dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existing.ImagePath) && System.IO.File.Exists(Path.Combine(_webHostEnvironment.WebRootPath , existing.ImagePath)))
                    {
                        _file.DeleteFile(existing.ImagePath , _webHostEnvironment.WebRootPath);
            
                    }
                    dto.ImagePath = await _file.SaveFileAsync(dto.ImageFile, _webHostEnvironment.WebRootPath, "images/products");
                }
                else
                {
                    dto.ImagePath = existing.ImagePath;
                    // اگر آپلود نشده، میتونیم ImagePath موجود را نگه داریم یا بر اساس dto تصمیم بگیریم
                    //dto.ImagePath = string.IsNullOrWhiteSpace(dto.ImagePath) ? existing.ImagePath : dto.ImagePath;
                }

                var product = await _productService.UpdateAsync(dto);
                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new ServiceResult(ResponseStatus.ServerError , null));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                return Ok(await _productService.DeleteAsync(id));
            }
            catch (Exception)
            {
                return BadRequest(new ServiceResult(ResponseStatus.ServerError, null));
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term , CancellationToken ct)
        {
            if(string.IsNullOrWhiteSpace(term))
            {
                return Ok(Array.Empty<ProductDto>());
            }

            var result = await _productService.SearchAsync(term, ct);
            return Ok(result);
        }

        //public IActionResult Chart()
        //{
        //    List<Product> pro = new List<Product>
        //   {
        //    new Product { Type = ProductType.Bracelet},
        //    new Product { Type = ProductType.TShirt},
        //    new Product { Type = ProductType.Thermos},
        //    new Product { Type = ProductType.SolarCharger},
        //    new Product { Type = ProductType.Lighter},
        //   };
        //    var brandCount = pro.GroupBy(b => b.Type).Select(g => new { Brand = g.Key, Count = g.Count() }).ToList();

        //    ViewBag.Labels = brandCount.Select(b => b.Brand.ToString()).ToList();
        //    ViewBag.Data = brandCount.Select(b => b.Count).ToList();
        //    return View();
        //}
    }
}
