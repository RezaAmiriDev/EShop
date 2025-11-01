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
        public async Task<CusProDto> GetByIdAsync(Guid Id, CancellationToken token = default)
        {
            try
            {
                var customerEntity = await _customerRepository.TableNoTracking
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == Id, token);

                if (customerEntity == null) return null!;
                //return _mapper.Map<CusProDto>(customerEntity);

                // استفاده از مپینگ دستی برای اطمینان
                var result = new CusProDto
                {
                    Id = customerEntity.Id,
                    Name = customerEntity.Name,
                    Family = customerEntity.Family,
                    Birth = customerEntity.Birth,
                    NationalCode = customerEntity.NationalCode,
                    CreateDate = customerEntity.CreateDate,
                    addressDto = customerEntity.Address == null ? null : new AddressDto
                    {
                        Id = customerEntity.Address.Id,
                        City = customerEntity.Address.City,
                        State = customerEntity.Address.State,
                        Tellphone = customerEntity.Address.Tellphone,
                        AdressDetail = customerEntity.Address.AdressDetail
                    }
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<CusProDto> GetByNationalCode(string NationalCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NationalCode))
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
                         Id = c.Address.Id,
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
        public async Task<PagedResponse<List<CusProDto>>> GetByPgination(PagedResponse<CusProDto> pageResponse, CancellationToken token = default)
        {
            try
            {
                // محافظت در برابر null
                if (pageResponse == null) throw new ArgumentNullException(nameof(pageResponse));
                var filterDto = pageResponse.Data ?? new CusProDto();

                var Query = _customerRepository.TableNoTracking.AsQueryable();

                if (!string.IsNullOrEmpty(filterDto.NationalCode))
                {
                    Query = Query.Where(d => d.NationalCode!.Contains(filterDto.NationalCode));
                }
                if (!string.IsNullOrEmpty(filterDto.Name))
                {
                    Query = Query.Where(d => d.Name!.Contains(filterDto.Name));
                }
                var Total = await Query.CountAsync(token);
                var list = await Query
                    .OrderByDescending(s => s.CreateDate)
                    .Skip(pageResponse.StartIndex)
                    .Take(pageResponse.PageSize)
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
                            Id = c.Address.Id,
                            City = c.Address.City,
                            State = c.Address.State,
                            Tellphone = c.Address.Tellphone,
                            AdressDetail = c.Address.AdressDetail,
                        }

                    }).ToListAsync(token);

                return new PagedResponse<List<CusProDto>>(pageResponse.PageNumber, Total, list);
            }
            catch (Exception)
            {
                throw new Exception(EnumExtention.GetEnumDescription(ResponseStatus.ServerError));
            }
        }
        public async Task<ServiceResult> CreateAsync(CusProDto customerDto, string UserID, CancellationToken token = default)
        {
            try
            {
                var existingCustomer = await _customerRepository.TableNoTracking.AnyAsync(
                    c => c.NationalCode == customerDto.NationalCode, token);
                if (existingCustomer)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "کد ملی تکراری است");
                }

                var customer = _mapper.Map<Customer>(customerDto);
                customer.CreateDate = DateTime.Now; // ✅ تنظیم تاریخ ایجاد

                if (customer.Address != null && (customer.Address.Id == Guid.Empty || customer.Address.Id == null))
                {
                    customer.Address.Id = Guid.NewGuid();
                }

                var result = await _customerRepository.AddAsync(customer, token);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAsync: {ex.Message}");
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }

        public async Task<ServiceResult> UpdateAsync(CusProDto customerDto, string UserID, CancellationToken token = default)
        {
            try
            {
                // بررسی وجود مشتری
                var existingCustomer = await _customerRepository.Table
                    .Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == customerDto.Id);
                if (existingCustomer == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, "مشتری یافت نشد");
                }

                var duplicateNationalCode = await _customerRepository.TableNoTracking
                    .AnyAsync(c => c.NationalCode == customerDto.NationalCode && c.Id != customerDto.Id, token);
                if (duplicateNationalCode)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "کد ملی تکراری است");
                }

                // به‌روزرسانی فیلدهای اصلی
                // مپ کردن فقط فیلدهای قابل تغییر
                existingCustomer.Name = customerDto.Name;
                existingCustomer.Family = customerDto.Family;
                existingCustomer.Birth = customerDto.Birth;
                existingCustomer.NationalCode = customerDto.NationalCode;

                // مدیریت آدرس
                if (customerDto.addressDto != null)
                {
                    if (existingCustomer.Address == null)
                    {
                        // ایجاد آدرس جدید
                        existingCustomer.Address = _mapper.Map<Address>(customerDto.addressDto);
                    }
                    else
                    {
                        // به‌روزرسانی آدرس موجود
                        existingCustomer.Address.City = customerDto.addressDto.City;
                        existingCustomer.Address.State = customerDto.addressDto.State;
                        existingCustomer.Address.Tellphone = customerDto.addressDto.Tellphone;
                        existingCustomer.Address.AdressDetail = customerDto.addressDto.AdressDetail;
                    }
                }

                var Result = await _customerRepository.UpdateAsync(existingCustomer, token);
                if (Result.Status == ResponseStatus.Success)
                {
                    return new ServiceResult(ResponseStatus.Success, "مشتری با موفقیت ویرایش شد");
                }

                return Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                return new ServiceResult(ResponseStatus.ServerError, "خطا در ویرایش مشتری");
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
