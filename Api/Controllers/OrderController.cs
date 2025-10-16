using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.ViewModel;
using ServiceLayer.Services;

namespace MobileStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _saleService;

        public OrderController(OrderService saleService)
        {
            _saleService = saleService;
        }

        //[HttpGet("{id:guid}")]
        //public async Task<IActionResult> GetById(Guid id)
        //{
        //    try
        //    {
        //        var result = await _saleService.GetByIdAsync(id); // فرض: ServiceResult با داده SaleDto
        //        if (result == null)
        //            return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, "سرویس پاسخی برنگرداند."));

        //        return MapServiceResult(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
        //    }
        //}

        /// ایجاد یک فروش جدید
        [HttpPost]
        public async Task<IActionResult> CreateSale(SaleDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var result = await _saleService.CreateSaleAsync(dto.CustomerId, dto.ProductId, dto.Quantity);

                // اگر سرویس null برگردوند خطای داخلی
                if (result == null)
                {
                    return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, "سرویس پاسخی برنگرداند."));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                // الگوی تو: در catch یک ServiceResult با کد سرور برگردانده می‌شود
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }
     
        // گزارش خرید مشتری
        [HttpGet("customer/{customerId:guid}")]
        public async Task<IActionResult> GetCustomerReport(Guid customerId)
        {
            try
            {
                var result = await _saleService.GetSalesByCustomerAsync(customerId);
                if (result == null)
                {
                    return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, "سرویس پاسخی برنگرداند."));
                }

                return Ok(result);
            }catch(Exception ex)
            {
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

        [HttpGet("product/{productId:guid}")]
        public async Task<IActionResult> GetProductReport(Guid productId)
        {
            try
            {
                var result = await _saleService.GetSalesByProductAsync(productId);
                if(result == null)
                {
                    return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, "سرویس پاسخی برنگرداند."));
                }

                return Ok(result);
            }catch(Exception ex)
            {
                return StatusCode(500 , new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotalSales()
        {
            try
            {
                var total = await _saleService.GetTotalSalesAsync();
                if (total == null)
                    return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, "سرویس پاسخی برنگرداند."));
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

    }
}
