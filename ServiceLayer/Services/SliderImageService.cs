using AutoMapper;
using DataLayer.ApiResult;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelLayer.Interface;
using ModelLayer.Models;
using ModelLayer.ViewModel;


namespace ServiceLayer.Services
{
    // Services/SliderService.cs
    public class SliderService 
    {
        private readonly ISliderRepository _repo;
        private readonly ILogger<SliderService>? _logger;
        private readonly IMapper _mapper;
        private readonly IFileRepository _file;

        public SliderService(ISliderRepository repo, ILogger<SliderService>? logger, IMapper mapper, IFileRepository fileRepository)
        {
            _logger = logger;
            _repo = repo;
            _mapper = mapper;
            _file = fileRepository;
        }


        public async Task<List<SliderImageDto>> GetAllAsync()
        {
            try
            {
                var items = await _repo.GetAllAsync();
                return _mapper.Map<List<SliderImageDto>>(items);
            }
            catch(Exception ex) 
            {
                _logger?.LogError(ex, "Error in SliderService.GetAllAsync");
                return new List<SliderImageDto>();
            }
        }

        public async Task<SliderImageDto> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null) return null!;

                return _mapper.Map<SliderImageDto>(entity);
            }
            catch(Exception ex)
            {
                _logger?.LogError(ex,"erorr in get all service !");
                return null!;
            }
          
        }
        public async Task<ServiceResult> CreateAsync(SliderImageDto slider, string webRootPath)
        {
            try
            {
                var entity = _mapper.Map<SliderImage>(slider);
                entity.Id = Guid.NewGuid();

                if(slider.Slider != null)
                {
                    var savedPath = await _file.SaveFileAsync(slider.Slider, webRootPath, "uploads/sliders");
                    entity.ImagePath = savedPath;
                }

                await _repo.AddAsync(entity);
                return new ServiceResult(ResponseStatus.Success, null);
            }catch(Exception ex)
            {
                _logger?.LogError(ex, "Error in CreateAsync");
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
        public Task UpdateAsync(SliderImage slider) => _repo.UpdateAsync(slider);
        public async Task<ServiceResult> DeleteAsync(Guid id, string webRootPath)
        {
            try
            {
                var exist = await _repo.GetByIdAsync(id);
                if (exist == null)
                {
                    return new ServiceResult(ResponseStatus.NotFound, null);
                }

                if (!string.IsNullOrWhiteSpace(exist.ImagePath))
                {
                    _file.DeleteFile(exist.ImagePath, webRootPath);
                }

                return await _repo.DeleteAsync(exist);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteAsync error: {ex.Message}");
                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
    }
}
