using Microsoft.AspNetCore.Mvc;
using ModelLayer.ViewModel;
using ServiceLayer.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;
        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {

                var result = await _dashboardService.GetSummaryAsync();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message =  ex.Message, detail = ex.ToString() });
            }
        }

        [HttpGet("daily-transactions")]
        public async Task<ActionResult<List<DailyTransactionDto>>> GeuTransaction([FromQuery] int days = 15)
        {
            try
            {
                return Ok(await _dashboardService.GetDailyTransactionsAsync(days));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("top-products")]
        public async Task<ActionResult<List<TopProductDto>>> GetTopProducts([FromQuery] int top = 6)
        {
            try
            {
                return Ok(await _dashboardService.GetTopProductsAsync(top));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("top-cities")]
        public async Task<ActionResult<List<CityOrderDto>>> GetTopCities([FromQuery] int top = 6)
        {
            try
            {
                return Ok(await _dashboardService.GetTopCitiesByOrdersAsync(top));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("")]
        public async Task<ActionResult<DashboardDto>> GetAll([FromQuery] int top = 6, [FromQuery] int days = 15)
        {
            try
            {
                return Ok(await _dashboardService.GetAllAsync(top, days));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
