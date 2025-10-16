using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using ModelLayer.ViewModel;
using ServiceLayer.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly SellerService _sellerService;

        public ShopController(SellerService sellerService)
        {
            _sellerService = sellerService;
        }

        [HttpGet]
        public async Task<IActionResult> GeyAll()
        {
            var items = await _sellerService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var seller = await _sellerService.GetByIdAsync(id);
            if(seller == null) return NotFound();
            return Ok(seller);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SellerDto seller)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _sellerService.CreateAsync(seller);
            if(result.Status == ResponseStatus.Success)
            {
                return StatusCode(201);
            }
            if(result.Status == ResponseStatus.BadRequest)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return StatusCode(500 , result.Status);
            }

        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, SellerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            dto.Id = id;
            var result = await _sellerService.UpdateAsync(dto);
            if (result.Status == ResponseStatus.Success)
                return NoContent();

            if (result.Status == ResponseStatus.NotFound)
                return NotFound(result.Message);

            if (result.Status == ResponseStatus.BadRequest)
                return BadRequest(result.Message);

            return StatusCode(500, result.Message);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _sellerService.DeleteAsync(id);
            if(result.Status == ResponseStatus.Success)
            {
                return NoContent();
            }
            if(result.Status == ResponseStatus.NotFound)
            {
                return NotFound(result.Message);
            }

            return StatusCode(500, result.Message);
        }

    }
}
