using AutoMapper;
using DataLayer.ApiResult;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Interface;
using ModelLayer.Models;
using ModelLayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class SellerService
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IMapper _mapper;

        public SellerService(ISellerRepository sellerRepository, IMapper mapper)
        {
            _sellerRepository = sellerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SellerDto>> GetAllAsync()
        {
            try
            {
                var list = await _sellerRepository.TableNoTracking
                    .Include(s => s.Address)
                    .Include(s => s.products)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<SellerDto>>(list);
            }
            catch (Exception)
            {
                // در صورت نیاز می‌توان لاگ اضافه کرد
                return Enumerable.Empty<SellerDto>();
            }
        }

        public async Task<SellerDto> GetByIdAsync(Guid Id)
        {
            try
            {
                var seller = await _sellerRepository.TableNoTracking
                    .Include(s => s.Address).Include(s => s.products)
                    .FirstOrDefaultAsync(s => s.Id == Id);
                return _mapper.Map<SellerDto>(seller);
            }catch(Exception)
            {
                return null!;
            }
        }

        public async Task<ServiceResult> CreateAsync(SellerDto dto)
        {
            try
            {
                var seller = _mapper.Map<Shop>(dto);
                if(seller.Id == Guid.Empty) seller.Id = Guid.NewGuid();
                return await _sellerRepository.AddAsync(seller);

            }
            catch (Exception)
            {
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }

        public async Task<ServiceResult> UpdateAsync(SellerDto dto)
        {
            try
            {
                if(dto.Id == null || dto.Id == Guid.Empty)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "Id is required for update");
                }
                var id = dto.Id.Value;

                var existing = await _sellerRepository.Table
                    .Include(s => s.products).FirstOrDefaultAsync(s => s.Id == id);

                if(existing == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, "Seller not found");
                }

                _mapper.Map(dto , existing);
                return await _sellerRepository.UpdateAsync(existing);
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
    }
}
