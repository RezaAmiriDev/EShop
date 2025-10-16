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


namespace ServiceLayer.Services
{
    public class ProductService 
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IHostEnvironment _env;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IMapper mapper, IHostEnvironment env, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _env = env;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductReadDto>> GetAllAsync(CancellationToken ct = default)
        {
            return await _productRepository.TableNoTracking
                .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);
        }

        public async Task<ProductReadDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var e = await _productRepository.GetByIdAsync(id);
            if (e == null) return null!;
            return _mapper.Map<ProductReadDto>(e);
        }

        public async Task<ServiceResult> CreateAsync(ProductCreateDto dto , CancellationToken ct = default)
        {
            try
            {
                var entity = _mapper.Map<Product>(dto);
                entity.Id = Guid.NewGuid();
                entity.DateOfOperation = DateTime.UtcNow;

                if(dto.ImageFile != null)
                {
                    var folder = Path.Combine(_env.ContentRootPath ?? "wwwroot", "uploads");
                    Directory.CreateDirectory(folder);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                    var path = Path.Combine(folder, fileName);
                    using var fs = System.IO.File.Create(path);
                    await dto.ImageFile.CopyToAsync(fs, ct);
                    entity.ImagePath = $"/uploads/{fileName}";
                }

                var result = await _productRepository.AddAsync(entity);
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
   
        public async Task<ServiceResult> UpdateAsync(ProductUpdateDto dto , CancellationToken ct = default)
        {
            try
            {
                var existing = await _productRepository.GetByIdAsync(dto.Id);
                if (existing == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, null);
                }

                _mapper.Map(dto, existing);

                if(dto.ImageFile != null)
                {
                    var folder = Path.Combine(_env.ContentRootPath ?? "wwwroot", "uploads");
                    Directory.CreateDirectory(folder);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ImageFile.FileName)}";
                    var path = Path.Combine(folder, fileName);
                    using var fs = System.IO.File.Create(path);
                    await dto.ImageFile.CopyToAsync(fs, ct);

                    if (!string.IsNullOrEmpty(existing.ImagePath))
                    {
                        try
                        {
                            var old = Path.Combine(_env.ContentRootPath ?? "wwwroot", existing.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                            if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                        }
                        catch { }
                    }

                    existing.ImagePath = $"/uploads/{fileName}";
                }
                var res = await _productRepository.UpdateAsync(existing);
                return new ServiceResult(ResponseStatus.Success, null);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating product {Id}", dto.Id);
                return new ServiceResult(ResponseStatus.NotFound, null);
            }
        }
 
        public async Task<ServiceResult> DeleteAsync(Guid id , CancellationToken ct = default)
        {
            try
            {
                var entity = await _productRepository.GetByIdAsync(id);
                if (entity != null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, null);
                }

                var res = await _productRepository.DeleteAsync(entity);
                if (res == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, null);
                }

                if (!string.IsNullOrEmpty(entity.ImagePath))
                {
                    try
                    {
                        var old = Path.Combine(_env.ContentRootPath ?? "wwwroot", entity.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                    }
                    catch { /* ignore */ }
                }
                return new ServiceResult(ResponseStatus.NotFound,null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {Id}", id);
                return new ServiceResult(ResponseStatus.ServerError , null);
            }
        }

        public async Task<IEnumerable<ProductReadDto>> SearchAsync(string trem , CancellationToken ct = default)
        {
            if(string.IsNullOrEmpty(trem)) return Array.Empty<ProductReadDto>();

            // اگر ProductRepo متدی به نام Search دارد از آن استفاده کن (بهینه)y
            if(_productRepository is ProductRepo pr)
            {
                var list = await pr.Search(trem); // returns List<Product>
                return _mapper.Map<List<ProductReadDto>>(list);
            }

            var list2 = await _productRepository.TableNoTracking
                .Where(p => EF.Functions.Like(p.Brand ?? string.Empty, $"%{trem}%"))
                .ProjectTo<ProductReadDto>(_mapper.ConfigurationProvider).ToListAsync();

            return list2;
        }
    }
}
