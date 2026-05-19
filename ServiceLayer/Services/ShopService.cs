using AutoMapper;
using ClassLibrary;
using Common.Pagination;
using DataLayer.ApiResult;
using DataLayer.EnumHellper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelLayer.Interface;
using ModelLayer.Models;
using ModelLayer.ViewModel;


namespace ServiceLayer.Services
{
    public class ShopService
    {
        private readonly IShopRepository _sellerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ShopService> _logger;

        public ShopService(IShopRepository sellerRepository, IMapper mapper, ILogger<ShopService> logger)
        {
            _sellerRepository = sellerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ShopDto>> GetAllAsync()
        {
            try
            {
                var list = await _sellerRepository.TableNoTracking
                    .Include(s => s.Address)
                    .Include(s => s.products)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<ShopDto>>(list);
            }
            catch (Exception ex)
            {
                // بهتر است لاگ بگیرید
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                // در صورت نیاز می‌توان لاگ اضافه کرد
                return Enumerable.Empty<ShopDto>();
            }
        }

        public async Task<ShopDto> GetByIdAsync(Guid Id)
        {
            try
            {
                var seller = await _sellerRepository.TableNoTracking
                    .Include(s => s.Address)
                    .Include(s => s.products)
                    .FirstOrDefaultAsync(s => s.Id == Id);
                if (seller == null) return null!;

                return _mapper.Map<ShopDto>(seller);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"GetByIdAsync error: {ex.Message}");
                return null!;
            }
        }

        public async Task<PagedResponse<List<ShopDto>>> GetByPgination(PagedResponse<ShopDto> paged)
        {
            try
            {
                if (paged == null) throw new ArgumentNullException(nameof(paged));
                var filter = paged.Data ?? new ShopDto();

                var Query = _sellerRepository.TableNoTracking.AsQueryable();

                if (!string.IsNullOrEmpty(filter.ShopCode))
                {
                    Query = Query.Where(d => d.ShopCode!.Contains(filter.ShopCode));
                }

                if (!string.IsNullOrEmpty(filter.ShopName))
                {
                    Query = Query.Where(d => d.ShopName!.Contains(filter.ShopName));
                }
                var Total = await Query.CountAsync();
                var list = await Query.Skip(paged.StartIndex)
                    .Take(paged.PageSize)
                    .Select(s => new ShopDto
                    {
                        Id = s.Id,
                        Description = s.Description,
                        ShopCode = s.ShopCode,
                        ImagePath = s.ImagePath,
                        AddressDto = s.Address == null ? null : new AddressDto
                        {
                            Id = s.Address.Id,
                            City = s.Address.City,
                            State = s.Address.State,
                            Tellphone = s.Address.Tellphone,
                            AdressDetail = s.Address.AdressDetail,
                        }


                    }).ToListAsync();

                return new PagedResponse<List<ShopDto>>(paged.PageNumber,paged.PageSize, Total, list);
            }
            catch (Exception)
            {
                throw new Exception(EnumExtention.GetEnumDescription(ResponseStatus.ServerError));
            }
        }

        public async Task<ServiceResult> CreateAsync(ShopDto dto, CancellationToken ct = default)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(dto.ShopCode))
                {
                    var code = dto.ShopCode.Trim();
                    var exists = await _sellerRepository.TableNoTracking
                        .AnyAsync(s => s.ShopCode == dto.ShopCode , ct);
                    if (exists)
                    {
                        return new ServiceResult(ResponseStatus.BadRequest, "کد فروشگاه قبلاً ثبت شده است.");
                    }
                }

                var seller = _mapper.Map<Shop>(dto);
                seller.Id = Guid.NewGuid();

                if(seller.Address != null && seller.Address.Id == Guid.Empty)
                {
                   seller.Address.Id = Guid.NewGuid();
                }

                var result = await _sellerRepository.AddAsync(seller , ct);
                return result ?? new ServiceResult(ResponseStatus.ServerError, null);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync failed");
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }

        public async Task<ServiceResult> UpdateAsync(ShopDto dto , CancellationToken ct = default)
        {
            try
            {
                if(dto.Id == null || dto.Id == Guid.Empty)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "Id is required for update");
                }
                var id = dto.Id.Value;

                var existing = await _sellerRepository.Table
                    .Include(s => s.products)
                    .Include(s => s.Address)
                    .FirstOrDefaultAsync(s => s.Id == dto.Id.Value, ct);

                if(existing == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, "Seller not found");
                }

                if (!string.IsNullOrWhiteSpace(dto.ShopCode))
                {
                    var code = dto.ShopCode.Trim();
                    var exists = await _sellerRepository.TableNoTracking
                        .AnyAsync(s => s.ShopCode == dto.ShopCode && s.Id != dto.Id, ct);
                    if (exists)
                    {
                        return new ServiceResult(ResponseStatus.BadRequest, "کد فروشگاه تکراری است.");
                    }
                }
                //// داخل ShopService.UpdateAsync
                //if (dto.ShopName != null) existing.ShopName = dto.ShopName;
                //if (dto.Description != null) existing.Description = dto.Description;
                //if (dto.ShopCode != null) existing.ShopCode = dto.ShopCode;
                //if (dto.ImagePath != null) existing.ImagePath = dto.ImagePath; // controller تنظیم می‌کند

                //if (dto.AddressDto != null)
                //{
                //    // اگر می‌خواهی آدرس جدید بسازی یا Id را ست کنی:
                //    if (dto.AddressDto.Id.HasValue && dto.AddressDto.Id.Value != Guid.Empty)
                //    {
                //        existing.AddressId = dto.AddressDto.Id.Value;

                //        // اگر موجودی دارای Address هست، آن را به روز کن، در غیر این صورت new Address
                //        if (existing.Address == null || existing.Address.Id != dto.AddressDto.Id.Value)
                //        {
                //            // اگر navigation موجود نیست یا Id فرق دارد، دستی یک navigation داشته باش
                //            existing.Address = existing.Address ?? new Address { Id = dto.AddressDto.Id.Value };
                //        }

                //        // اگر navigation موجود، فیلدهایش را آپدیت کن
                //        if (dto.AddressDto.City != null) existing.Address.City = dto.AddressDto.City;
                //        if (dto.AddressDto.State != null) existing.Address.State = dto.AddressDto.State;
                //        if (dto.AddressDto.Tellphone != null) existing.Address.Tellphone = dto.AddressDto.Tellphone;
                //        if (dto.AddressDto.AdressDetail != null) existing.Address.AdressDetail = dto.AddressDto.AdressDetail;
                //    }
                //    else
                //    {
                //        // 2) اگر Id ارسال نشده اما فیلدهای آدرس وجود دارد => آدرس جاری را آپدیت کن (اگر ندارد بساز)
                //        if (existing.Address == null) existing.Address = new Address { Id = Guid.NewGuid() };
                //        if (dto.AddressDto.City != null) existing.Address.City = dto.AddressDto.City;
                //        if (dto.AddressDto.State != null) existing.Address.State = dto.AddressDto.State;
                //        if (dto.AddressDto.Tellphone != null) existing.Address.Tellphone = dto.AddressDto.Tellphone;
                //        if (dto.AddressDto.AdressDetail != null) existing.Address.AdressDetail = dto.AddressDto.AdressDetail;
                //        // sync FK
                //        existing.AddressId = existing.Address.Id;
                //    }
                //}

                _mapper.Map(dto , existing);
                return await _sellerRepository.UpdateAsync(existing, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync failed for Shop {Id}", dto.Id);
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }

        public async Task<ServiceResult> DeleteAsync(Guid id)
        {
            try
            {
                var exist = await _sellerRepository.Entities.AnyAsync(d => d.Id == id);
                if (exist)
                {
                    var seller = new Shop { Id = id };
                    return await _sellerRepository.DeleteAsync(seller);
                }
                else
                {
                    return new ServiceResult(ResponseStatus.NotFound, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteAsync error: {ex.Message}");
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }

        public async Task<ShopDto?> IncrementLikeAsync(Guid id)
        {
            try
            {
                try
                {
                    var effected = await _sellerRepository.Table
                        .Where(s => s.Id == id)
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.LikesCount, p => p.LikesCount + 1));

                    if (effected == 0) return null;
                }
                catch
                {
                    var existingFallBack = await _sellerRepository.Table.FirstOrDefaultAsync(s => s.Id == id);
                    if (existingFallBack == null) return null;
                    existingFallBack.LikesCount++;
                    await _sellerRepository.UpdateAsync(existingFallBack);
                }
                // بازخوانی موجود و تبدیل به DTO برای بازگرداندن شمارش‌های فعلی
                var updated = await _sellerRepository.TableNoTracking.FirstOrDefaultAsync(s => s.Id == id);

                return updated == null ? null : _mapper.Map<ShopDto>(updated);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IncrementLikeAsync error: {ex.Message}");
                return null;
            }
        }

        public async Task<ShopDto> IncrementDislikeAsync(Guid id)
        {
            try
            {
                try
                {
                    var effected = await _sellerRepository.Table.Where(s => s.Id == id)
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.DislikesCount, p => p.DislikesCount + 1));

                    if (effected == 0) return null!;
                }
                catch
                {
                    var existingFallback = await _sellerRepository.Table.FirstOrDefaultAsync(s => s.Id == id);
                    if (existingFallback == null) return null!;
                    existingFallback.DislikesCount++;
                    await _sellerRepository.UpdateAsync(existingFallback);
                }

                var updated = await _sellerRepository.TableNoTracking.FirstOrDefaultAsync(s => s.Id == id);

                return updated == null ? null : _mapper.Map<ShopDto>(updated);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IncrementDislikeAsync error: {ex.Message}");
                return null!;
            }
        }

        // Optional helper: compute trust percent (0-100)
        public int ComputeTrustPercent(int likes , int dislikes)
        {
            var total = likes + dislikes;
            if(total == 0) return 100;
            return (int)Math.Round((double)likes / total * 100);
        }
    }
}
