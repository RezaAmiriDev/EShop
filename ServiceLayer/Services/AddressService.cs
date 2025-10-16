using AutoMapper;
using ClassLibrary;
using ClassLibrary.Repository;
using DataLayer.ApiResult;
using Microsoft.EntityFrameworkCore;
using System;


namespace ServiceLayer.Services
{
    public class AddressService
    {
        private readonly IAdressRepository _adressRepository;
       

        public AddressService(IAdressRepository adressRepository, IMapper mapper)
        {
            _adressRepository = adressRepository;
          
        }

        public async Task<IEnumerable<Address>> GetAll()
        {
            try
            {

              return await _adressRepository.Entities.ToListAsync();

            }catch (Exception)
            {
                return null!;
            }
           
        }

        public async Task<Address> GetById(Guid Id)
        {
            try
            {
                return await _adressRepository.GetByIdAsync(Id);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<ServiceResult> Create(Address address)
        {
            try
            {

                await _adressRepository.AddAsync(address);
                return new ServiceResult(ResponseStatus.Success, null);

            }
            catch (Exception)
            {
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
      
        public async Task<ServiceResult> Update(Address address)
        {
            try
            {
                return await _adressRepository.UpdateAsync(address);
            }
            catch(Exception)
            {
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
           
        }
 
        public async Task<ServiceResult> Delete(Guid id)
        {
            try
            {
                var ExistUser = await _adressRepository.Table.AnyAsync(x => x.Id == id);
                if (ExistUser)
                {
                    var address = new Address { Id = id };
                    return await _adressRepository.DeleteAsync(address);
                }
                else
                {
                    return new ServiceResult(ResponseStatus.NotFound, null);
                }
                
            }
            catch (Exception)
            {
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
            
        }
    
    }
}
