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

        public async Task<HomeDto> GetHomeAsync(int take = 12, string? userIdOrTempId = null, CancellationToken ct = default)
        {
            // دریافت محصولات (همراه با نام فروشنده، امتیاز و ...)
            var products = await _productService.GetProductsForHomeAsync(take);

            // دریافت اسلایدرهای فعال (نه همه)
            var activeSlider = await _sliderService.GetActiveSliderAsync();
            var sliders = _mapper.Map<List<SliderImageDto>>(activeSlider);
           

            var home = new HomeDto
            {
                ProductItm = products ?? new List<ProductCardDto>(),
                SliderImg = sliders ?? new List<SliderImageDto>(),
                CartItemCount = 0,
            };

            return home;
        }
    }
}
