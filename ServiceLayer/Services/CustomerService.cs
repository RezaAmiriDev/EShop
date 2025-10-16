using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using ClassLibrary;
using ClassLibrary.Models;
using ClassLibrary.Repository;
using ClassLibrary.ViewModel;
using Common.Pagination;
using DataLayer.ApiResult;
using DataLayer.EnumHellper;
using Microsoft.EntityFrameworkCore;
using ModelLayer.ViewModel;



namespace ServiceLayer.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CusProDto>> GetAll()
        {
            try
            {
                var list = await _customerRepository.TableNoTracking
           .ProjectTo<CusProDto>(_mapper.ConfigurationProvider)
           .ToListAsync();

                return list;

            }
            catch (Exception)
            {
                return Enumerable.Empty<CusProDto>();
            }
        }
        public async Task<CusProDto> GetById(Guid Id)
        {
            try
            {

                var customer = await _customerRepository.GetByIdAsync(Id);
                return _mapper.Map<CusProDto>(customer);
            }
            catch (Exception)
            {
                return null!;
            }
        }
        //public async Task<CusProDto> GetCustomerById(string Id)
        //{
        //    try
        //    {
        //        var customer = await _customerRepository.TableNoTracking.Include(c => c.AddressId)
        //            .Select(c => new CusProDto
        //            {
        //                Id = c.Id,
        //                Name = c.Name,
        //                NationalCode = c.NationalCode,
        //                CreateDate = c.CreateDate,
        //                Address = c.Address!,
        //                Family = c.Family,
        //            }).FirstOrDefaultAsync(d => d.Id == new Guid(Id));
        //        return customer!;

        //    }
        //    catch (Exception)
        //    {
        //        throw new Exception("خطا در دریافت اطلاعات مشتری");
        //    }
        //}
        public async Task<CusProDto> GetByNationalCode(string NationalCode)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(NationalCode))
                    return null!;

               var customer = await _customerRepository.TableNoTracking
                .Where(c => c.NationalCode == NationalCode)
                .Select(c => new CusProDto
                {
                    Id = c.Id,
                    NationalCode = c.NationalCode,
                    Name = c.Name,
                    Family = c.Family,
                    Birth = c.Birth,
                    CreateDate = c.CreateDate,
                    addressDto = c.Address == null ? null : new AddressDto
                    {
                        Id = c.Id,
                        City = c.Address.City,
                        State = c.Address.State,
                        Tellphone = c.Address.Tellphone,
                        AdressDetail = c.Address.AdressDetail,
                    } 
                }).FirstOrDefaultAsync();

                return customer!;
            }
            catch (Exception ex)
            {
                throw new Exception("خطا در دریافت مشتری بر اساس کد ملی", ex);
            }
        }
        public async Task<PagedResponse<IEnumerable<CusProDto>>> GetByPgination(PagedResponse<CusProDto> pageResponse)
        {
            try
            {
                // محافظت در برابر null
                var filterDto = pageResponse?.Data ?? new CusProDto();

                var Query = _customerRepository.TableNoTracking.AsQueryable();

                if (!string.IsNullOrEmpty(filterDto.NationalCode))
                {
                    Query = Query.Where(d => d.NationalCode.Contains(filterDto.NationalCode));
                }
                if (!string.IsNullOrEmpty(filterDto.Name))
                {
                    Query = Query.Where(d => d.Name.Contains(filterDto.Name));
                }
                var Total = await Query.CountAsync();
                var list = await Query.Select(c => new CusProDto
                {
                    Id = c.Id,
                    NationalCode = c.NationalCode,
                    Name = c.Name,
                    Family = c.Family,
                    Birth = c.Birth,
                    CreateDate = c.CreateDate,
                    addressDto = c.Address == null ? null : new AddressDto
                    {
                        Id = c.Address.Id,
                        City = c.Address.City,
                        State = c.Address.State,
                        Tellphone = c.Address.Tellphone,
                        AdressDetail = c.Address.AdressDetail,
                    }
                   
                }).OrderByDescending(s => s.CreateDate).Skip(pageResponse.StartIndex)
                .Take(pageResponse.PageSize).ToListAsync();

                return new PagedResponse<IEnumerable<CusProDto>>(pageResponse.PageNumber, Total, list);
            }
            catch (Exception)
            {
                throw new Exception(EnumExtention.GetEnumDescription(ResponseStatus.ServerError));
            }
        }
        public async Task<ServiceResult> Create(CusProDto customerDto, string UserID)
        {
            try
            {
                var customer = _mapper.Map<Customer>(customerDto);
                var result = await _customerRepository.AddAsync(customer);
                return result;
            }
            catch (Exception)
            {
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
        public async Task<ServiceResult> Update(CusProDto customerDto, string UserID)
        {
            try
            {
                var customer = _mapper.Map<Customer>(customerDto);
                var Result = await _customerRepository.UpdateAsync(customer);
                return Result;
            }
            catch (Exception)
            {
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
        public async Task<ServiceResult> Delete(Guid Id, string UserID)
        {
            try
            {
                var ExistCustomer = await _customerRepository.Entities.AnyAsync(d => d.Id == Id);
                if (ExistCustomer)
                {
                    var customer = _mapper.Map<Customer>(new CusProDto { Id = Id });
                    var result = await _customerRepository.DeleteAsync(customer);
                    return result;
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
        // GetExistCustomer → برای ثبت مشتری جدید.
        public async Task<ServiceResult> ExistCustomer(string NationalCode)
        {
            try
            {
                var Exist = await _customerRepository.TableNoTracking.AnyAsync(d => d.NationalCode == NationalCode);
                if (Exist)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "شناسه وارد شده  تکراری است ");
                }
                else
                {
                    return new ServiceResult(ResponseStatus.Success, null);
                }
            }
            catch (Exception)
            {
                return new ServiceResult(ResponseStatus.BadRequest, null);
            }
        }
        // ExistCustomerUpdate → برای ویرایش مشتری موجود.
        public async Task<ServiceResult> ExistCustomerUpdate(string NationalCode, string CustomerId)
        {
            try
            {
                var Exist = await _customerRepository.TableNoTracking.AnyAsync(d => d.NationalCode == NationalCode && d.Id != new Guid(CustomerId));
                if (Exist)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "شناسه وارد شده  تکراری است ");
                }
                else
                {
                    return new ServiceResult(ResponseStatus.Success, null);
                }
            }
            catch (Exception)
            {
                return new ServiceResult(ResponseStatus.BadRequest, null);
            }
        }


    }
}
