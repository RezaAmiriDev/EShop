using ClassLibrary;
using ClassLibrary.Models;
using ClassLibrary.Repository;
using ClassLibrary.ViewModel;
using Common.Pagination;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services;


namespace MobileStore.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _customerService.GetAll();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500 , new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var customer = await _customerService.GetById(id);
                if (customer == null) return NotFound();
                return Ok(customer);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

        [HttpGet("by-code/{code}")]
        public async Task<IActionResult> GetByNationalCode(string code)
        {
            try
            {
                var customer = await _customerService.GetByNationalCode(code);
                if (customer != null)
                {
                    return Ok(customer);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetByPaginationCustomer(PagedResponse<CusProDto> pagedResponse)
        {
            try
            {
                if(pagedResponse == null)
                {
                    return BadRequest(new ServiceResult(ResponseStatus.BadRequest, "Invalid request payload"));
                }

                var result = await _customerService.GetByPgination(pagedResponse);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }
      
        // POST: CustomerController/Create
        [HttpPost]
        public async Task<IActionResult> Create(CusProDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _customerService.Create(dto, User?.Identity?.Name ?? "system");
                if (result.Status == ResponseStatus.Success) return Ok(result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

        // POST: CustomerController/Edit/5
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Edit(Guid id, CusProDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                //dto.Id = new Guid(id);
                var result = await _customerService.Update(dto, User?.Identity?.Name ?? "system");
                if (result.Status == ResponseStatus.Success) return Ok(result);
                if (result.Status == ResponseStatus.NotFound) return NotFound(result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

        // POST: CustomerController/Delete/
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var userId = User?.Identity?.Name ?? "system"; // یا User.Identity.Name بسته به نیاز
                var res = await _customerService.Delete(id, userId);

                return res.Status switch
                {
                    ResponseStatus.Success => Ok(res),
                    ResponseStatus.NotFound => NotFound(res),
                    _ => BadRequest(res)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }
    }

}

