using ClassLibrary;
using DataLayer.ApiResult;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services;

namespace Api.Controllers
{
    public class AddressController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task <IActionResult> GetAll()
        {
            try
            {
                return Ok(await _addressService.GetAll());

            }catch (Exception)
            {
                return BadRequest(new ServiceResult(ResponseStatus.ServerError , null));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid Id)
        {
            try
            {
                var address = await _addressService.GetById(Id);
                if (address != null)
                {
                    return Ok(address);
                }
                else
                {
                    return Ok(new ServiceResult(ResponseStatus.NotFound, null));
                }
            }catch (Exception)
            {
                return Ok(new ServiceResult(ResponseStatus.ServerError, null));
            }
        }
       
        [HttpPost]
        public async Task<IActionResult> Create(Address address)
        {
            try
            {
                var Result = await _addressService.Create(address);
                if(Result.Status == ResponseStatus.Success)
                {
                    return BadRequest(Result);
                }
                return Ok(Result);
            }
            catch (Exception)
            {
                return BadRequest(new ServiceResult(ResponseStatus.ServerError, null)); 
            }
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(Guid Id)
        {
            try
            {
                return Ok(await _addressService.Delete(Id));

            }catch(Exception)
            {
                return BadRequest(new ServiceResult(ResponseStatus.ServerError, null));
            }
        }
    
        [HttpPut]
        public async Task<IActionResult> Update(Address address)
        {
            try
            {
                return Ok(await _addressService.Update(address));
            }
            catch (Exception)
            {
                return BadRequest(new ServiceResult(ResponseStatus.ServerError, null));
            }
        }
    }
}
