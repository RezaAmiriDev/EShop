

namespace ModelLayer.ViewModel
{

    public class ProductCardDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public string ShopName { get; set; }   // نام فروشنده

        //  public string ShopCode { get; set; }   // برای لینک به صفحه فروشگاه
        public double AverageRating { get; set; } // میانگین امتیازات
        public bool CurrentUserLiked { get; set; } // آیا کاربر فعلی به این محصول امتیاز داده؟
    }

    public class ProductDetailDto : ProductCardDto
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public string? ProductCode { get; set; }
        public string ImagePath { get; set; }
        public double AverageRating { get; set; } // میانگین امتیازات
        public string? ShortDescription { get; set; }

        public string ShopName { get; set; }   // نام فروشنده
        public string ShopCode { get; set; }   // برای لینک به صفحه فروشگاه
        public string? Description { get; set; }
        public AddressDto ShopAddress { get; set; }
        public List<RatingDto> Rating { get; set; }
    }

    public class HomeDto
    {
        public List<ProductCardDto>? ProductItm { get; set; } = new();
        public List<SliderImageDto>? SliderImg { get; set; } = new();
        public int CartItemCount { get; set; }
    }
}
