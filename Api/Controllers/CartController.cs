using ClassLibrary;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.ViewModel;
using ServiceLayer.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {

        private readonly CartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(CartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart(string userId, CancellationToken token)
        {
            try
            {
                var customerId = await _cartService.ResolveCustomerIdAsync(userId, User);
                if(!customerId.HasValue) return BadRequest("Invalid user identifier");

                var result = await _cartService.GetCartAsync(customerId.Value, token);
                return Ok(result);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error get cart");
                return StatusCode(500);
            }
        }


        [HttpGet("count")]
        public async Task<IActionResult> GetCartCount(string userId, CancellationToken token)
        {
            try
            {
                var customerId = await _cartService.ResolveCustomerIdAsync(userId, User);
                if(!customerId.HasValue) return BadRequest("Invalid user identifier");

                var result = await _cartService.GetCartCountAsync(customerId.Value, token);
                if (result.Status != ResponseStatus.Success) { return BadRequest(result); }
                return Ok(result);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error get cart count");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartDto dto, CancellationToken token)
        {
            try
            {
                if(dto.CustomerId == null) return BadRequest("CustomerId is required");

                var result = await _cartService.AddToCartAsync(dto, token);
                if(result.Status != ResponseStatus.Success) { return BadRequest(result); }
                return Ok(result);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "Error add to cart");
                return StatusCode(500);
            }
        }

        [HttpDelete("{userId}/{productId}")]
        public async Task<IActionResult> RemoveFromCart(string userId, Guid productId, CancellationToken token)
        {
            try
            {
                var customerID = await _cartService.ResolveCustomerIdAsync(userId, User);
                if(!customerID.HasValue) return BadRequest("Invalid user identifier");

                var result = await _cartService.RemoveFromCartAsync(customerID.Value, productId, token);
                if (result.Status != ResponseStatus.Success) { return BadRequest(result); }
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error remove from cart");
                return StatusCode(500);
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart(string userId, CancellationToken token)
        {
            try
            {
                var customerId = await _cartService.ResolveCustomerIdAsync(userId, User);
                if(customerId.HasValue) return BadRequest("Invalid user identifier");

                var result = await _cartService.ClearCartAsync(customerId.Value, token);
                if (result.Status != ResponseStatus.Success) { return BadRequest(result); }
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error clear art");
                return StatusCode(500);
            }
           
        }
    }
}


