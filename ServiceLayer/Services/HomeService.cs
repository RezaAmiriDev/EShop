using AutoMapper;
using ModelLayer.ViewModel;

namespace ServiceLayer.Services
{
    public class HomeService
    {
        private readonly ProductService _productService;
        private readonly SliderService _sliderService;
        private readonly IMapper _mapper;

        public HomeService(ProductService productService, SliderService sliderService, IMapper mapper)
        {
            _productService = productService;
            _sliderService = sliderService;
            _mapper = mapper;
        }

        public async Task<HomeDto> GetHomeAsync(int take = 12)
        {
            var products = await _productService.GetProductsForHomeAsync(take);
            var sliderDomain = await _sliderService.GetAllAsync();
            var sliders = _mapper.Map<List<SliderImageDto>>(sliderDomain);
           

            var home = new HomeDto
            {
                ProductItm = products ?? new List<ProductListItemDto>(),
                SliderImg = sliders ?? new List<SliderImageDto>()
            };

            return home;
        }
    }
}
