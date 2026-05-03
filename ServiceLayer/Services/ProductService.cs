using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClassLibrary;
using ClassLibrary.Repository;
using ClassLibrary.Services;
using DataLayer.ApiResult;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelLayer.ViewModel;
using Microsoft.Extensions.Hosting;
using Common.Pagination;
using DataLayer.EnumHellper;


namespace ServiceLayer.Services
{
    public class ProductService 
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IMapper mapper, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default)
        {
            return await _productRepository.TableNoTracking
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);
        }

        public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                var e = await _productRepository.GetByIdAsync(id, ct);
                if (e == null) return null!;
                return _mapper.Map<ProductDto>(e);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Get By ID In ProductService Err : {ex.Message}");
                return null!;
            }
            
        }

        public async Task<PagedResponse<List<ProductDto>>> GetByPgination(PagedRequest<ProductDto> pagedResponse)
        {
            try
            {
                if (pagedResponse == null) throw new ArgumentException(nameof(pagedResponse));

                var filterDto = pagedResponse.Data ?? new ProductDto();
                var query = _productRepository.TableNoTracking.AsQueryable();

                if (!string.IsNullOrEmpty(filterDto.Brand))
                {
                    query = query.Where(p => p.Brand!.Contains(filterDto.Brand));
                }

                if (!string.IsNullOrEmpty(filterDto.ProductCode))
                {
                    query = query.Where(p => p.ProductCode!.Contains(filterDto.ProductCode));
                }

                var Total = await query.CountAsync();
                var list = await query.Skip(pagedResponse.StartIndex)
                    .Take(pagedResponse.PageSize).Select(p => new ProductDto
                    {
                        Id = p.Id,
                        Brand = p.Brand,
                        ProductCode = p.ProductCode,
                        Type = p.Type,
                        ImagePath = p.ImagePath,
                        Price = p.Price
                    }).ToListAsync();
                return new PagedResponse<List<ProductDto>>(pagedResponse.PageNumber,pagedResponse.PageSize, Total, list);
            }
            catch(Exception)
            {
                throw new Exception(EnumExtention.GetEnumDescription(ResponseStatus.ServerError));
            }
        }

        public async Task<ServiceResult> CreateAsync(ProductDto dto , CancellationToken ct = default)
        {
            try
            {
                var product = _mapper.Map<Product>(dto);
                product.Id = Guid.NewGuid();
                product.DateOfOperation = DateTime.UtcNow;

                var result = await _productRepository.AddAsync(product , ct);
                if (result == null)
                {
                    _logger.LogWarning("Create product failed: {@Result}", result);
                    return new ServiceResult(ResponseStatus.BadRequest, null);
                }

                return new ServiceResult(ResponseStatus.Success , null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
   
        public async Task<ServiceResult> UpdateAsync(ProductDto dto , CancellationToken ct = default)
        {
            try
            {
                if(dto.Id == null || dto.Id == Guid.Empty)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, "شناسه محصول نامعتبر است.");
                }

                var existing = await _productRepository.GetByIdAsync(dto.Id.Value , ct);
                if (existing == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, null);
                }

                _mapper.Map(dto, existing);

                var res = await _productRepository.UpdateAsync(existing , ct);

                return new ServiceResult(ResponseStatus.ServerError, "خطا در به‌روزرسانی محصول");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating product {Id}", dto.Id);
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
 
        public async Task<ServiceResult> DeleteAsync(Guid id , CancellationToken ct = default)
        {
            try
            {
                var entity = await _productRepository.GetByIdAsync(id , ct);
                if (entity == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, "محصول یافت نشد.");
                }

                var res = await _productRepository.DeleteAsync(entity, ct);
                if (res != null) return res;

                return new ServiceResult(ResponseStatus.NotFound,null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {Id}", id);
                return new ServiceResult(ResponseStatus.ServerError , null);
            }
        }

        public async Task<IEnumerable<ProductDto>> SearchAsync(string term , CancellationToken ct = default)
        {
            if(string.IsNullOrEmpty(term)) return Array.Empty<ProductDto>();

            var list = await _productRepository.TableNoTracking
                .Where(p => EF.Functions.Like(p.Brand ?? string.Empty  , $"%{term}%"))
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider).ToListAsync(ct);

            return list;
        }

        public async Task<List<ProductListItemDto>> GetProductsForHomeAsync(int take = 12, CancellationToken ct = default)
        {
            var query = _productRepository.TableNoTracking
                .Select(p => new ProductListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Brand = p.Brand,
                    ProductCode = p.ProductCode,
                    ImagePath = p.ImagePath,
                    Price = p.Price,
                    ShortDescription = p.ShortDescription,
                    AverageRating = p.Ratings!
                    .Where(r => r.IsApproved).Select(r => (double)r.Review).DefaultIfEmpty(0.0).Average(),
                    ShopName = p.sellers!.OrderBy(s => s.ShopName).Select(s => s.ShopName).FirstOrDefault()
                }).OrderBy(p => p.Name).Take(take);

            return await query.ToListAsync(ct);
        }

        //public async Task<List<ProductListItemDto>> GetProductsForHomeAsync(int take = 12, CancellationToken ct = default)
        //{
        //    return await _productRepository.TableNoTracking
        //               .ProjectTo<ProductListItemDto>(_mapper.ConfigurationProvider)
        //               .OrderBy(p => p.Name)
        //               .Take(take)
        //               .ToListAsync(ct);
        //}
    }
}
