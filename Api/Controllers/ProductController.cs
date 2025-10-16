using ClassLibrary.Services;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.ViewModel;
using ServiceLayer.Services;


namespace MobileStore.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductRepo> _logger;
        public ProductController(ProductService productService, IWebHostEnvironment webHostEnvironment, ILogger<ProductRepo> logger)
        {
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
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

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            try
            {
                var product = await _productService.CreateAsync(dto);
                if (product.Status == ResponseStatus.Success)
                {
                    return BadRequest(product);
                }
                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new ServiceResult(ResponseStatus.ServerError, null));
            }
            //if(product.ImageFile == null || product.ImageFile.Length == 0)
            //{
            //   ModelState.AddModelError("ImageFile", "لطفاً یک تصویر انتخاب کنید");
            //}
            //if (!ModelState.IsValid)
            //{
            //    return View(product);
            //}
            //string uniqueFileName = null!;

            ////save image file   ذخیره فایل تصویر
            //if(product.ImageFile != null)
            //{
            //    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/products");
            //    if (!Directory.Exists(uploadsFolder))
            //    {
            //        Directory.CreateDirectory(uploadsFolder);
            //    }

            //    uniqueFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(product.ImageFile.FileName);
            //    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            //    // چاپ مسیر فایل برای بررسی
            //    Console.WriteLine("File saved to: " + filePath);

            //    // ذخیره فایل در مسیر مشخص‌شده
            //    using (var fileStream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await product.ImageFile.CopyToAsync(fileStream);
            //    }
            //}
            //// ذخیره اطلاعات محصول در دیتابیس
            //Product newProduct = new Product()
            //{
            //    Brand = product.Brand,
            //    Type = product.Type,
            //    ImagePath = "/uploads/products/" + uniqueFileName, // ذخیره مسیر نسبی
            //    Price = product.Price
            //};

            //_context.Products.Add(newProduct);
            //await _context.SaveChangesAsync();

            //return RedirectToAction("Index", "Product");
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, ProductUpdateDto dto)
        {
            try
            {
                var product = await _productService.UpdateAsync(dto);
                if (id != dto.Id)
                {
                    return BadRequest("Id mismatch");
                }
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
