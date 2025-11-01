using ClassLibrary.ViewModel;
using Common.Pagination;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MobileStore.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(CustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService =  customerService;
            _logger = logger;
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
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken token = default)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id, token);
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
        public async Task<IActionResult> GetByPaginationCustomer(Common.Pagination.PagedResponse<CusProDto> req , CancellationToken token = default)
        {
            try
            {
                if (req == null)
                {
                    return BadRequest(new ServiceResult(ResponseStatus.BadRequest, "Invalid request payload"));
                }
                var pageNumber = req.PageNumber <= 0 ? 1 : req.PageNumber;
                var pageSize = req.PageSize <= 0 ? 10 : req.PageSize;

                var temp = new Common.Pagination.PagedResponse<CusProDto>(pageNumber, 0, req.Data ?? new CusProDto());
                // اما چون ctor PagedResponse شما بعضی چیزها را خودِ سرویس محاسبه می‌کند،
                // سرویس شما می‌تواند فقط از فیلدهای temp.StartIndex و temp.Data استفاده کند.

                // فراخوانی سرویس (منطق صفحه‌بندی داخل سرویس شما است)
                var servicePaged = await _customerService.GetByPgination(temp, token);
                // servicePaged: PagedResponse<List<CusProDto>>
                // تبدیل سرویس به PagedResult (با استفاده از ctor که اضافه کردیم)
                var outPaged = new Common.Pagination.PagedResult<List<CusProDto>>(servicePaged);
                return Ok(servicePaged);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServiceResult(ResponseStatus.ServerError, ex.Message));
            }
        }

        // POST: CustomerController/Create
        [HttpPost]
        public async Task<IActionResult> Create(CusProDto dto, CancellationToken token)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _customerService.CreateAsync(dto, User?.Identity?.Name ?? "system", token);
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
        public async Task<ActionResult> Edit(Guid id, CusProDto dto, CancellationToken token = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                   var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                   _logger.LogWarning("Model validation failed for customer update: {@Errors}", errors);
                    return BadRequest(ModelState);
                }

                // مطمئن شویم که Id در DTO با Id در route مطابقت دارد
                if (dto.Id != id)
                {
                    _logger.LogWarning("ID mismatch in customer update. Route ID: {RouteId}, DTO ID: {DtoId}", id, dto.Id);
                    return BadRequest(new ServiceResult(ResponseStatus.BadRequest, "عدم تطابق شناسه مشتری"));
                }

                //dto.Id = new Guid(id);
                var result = await _customerService.UpdateAsync(dto, User?.Identity?.Name ?? "system", token);
                if (result.Status == ResponseStatus.Success) return Ok(result);
                if (result.Status == ResponseStatus.NotFound) return NotFound(result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create customer failed. DTO: {@dto}", dto);
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

