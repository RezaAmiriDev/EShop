using Common.Pagination;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.ViewModel;
using ServiceLayer.Services;

namespace Api.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrderController(OrderService orderService) 
        {
            _orderService = orderService; 
        }
      
        [HttpGet("customer/{customerId:guid}")]
        public async Task<IActionResult> GetCustomerReport(Guid customerId)
        {
            try
            {
                var result = await _orderService.GetSalesByCustomerAsync(customerId);
                return StatusCode((int)MapStatusToCode(result.Status), result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResultByData<List<OrderDto>>(ResponseStatus.ServerError, ex.Message, null!));
            }
        }

        [HttpGet("product/{productId:guid}")]
        public async Task<IActionResult> GetProductReport(Guid productId)
        {
            try
            {
                var result = await _orderService.GetSalesByProductAsync(productId);
                return StatusCode((int)MapStatusToCode(result.Status), result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResultByData<List<OrderDto>>(ResponseStatus.ServerError, ex.Message, null!));
            }
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotalSales()
        {
            try
            {
                var result = await _orderService.GetTotalSalesAsync();
                return StatusCode((int)MapStatusToCode(result.Status), result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResultByData<decimal>(ResponseStatus.ServerError, ex.Message, 0m));
            }
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetRecentPayments(int page = 1, int pageSize = 12)
        {
            try
            {
                var request = new PagedRequest<PaymentDto>
                {
                    PageNumber = page,
                    PageSize = pageSize,
                    StartIndex = (page - 1) * pageSize
                };
                var result = await _orderService.GetRecentPaymentsAsync(request); 
                return StatusCode((int)MapStatusToCode(result.Status), result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResultByData<PagedResponse<List<PaymentDto>>>(ResponseStatus.ServerError, ex.Message, null!));
            }
        }

        private static System.Net.HttpStatusCode MapStatusToCode(ResponseStatus status)
        {
            return status switch
            {
                ResponseStatus.Success => System.Net.HttpStatusCode.OK,
                ResponseStatus.BadRequest => System.Net.HttpStatusCode.BadRequest,
                ResponseStatus.NotFound => System.Net.HttpStatusCode.NotFound,
                _ => System.Net.HttpStatusCode.InternalServerError
            };
        }
    }
}