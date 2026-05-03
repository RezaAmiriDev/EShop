using AutoMapper;
using ClassLibrary;
using ClassLibrary.ViewModel;
using ModelLayer.Models;
using ModelLayer.ViewModel;

namespace WebFrameWork.Mapper
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {

            // Address ⇄ AddressDto
            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>()
                .ForMember(dest => dest.Customers, opt => opt.Ignore());

            // Customer ⇄ CusProDto
            CreateMap<Customer, CusProDto>().ReverseMap()
                .ForMember(dest => dest.Address , opt => opt.MapFrom(src => src.addressDto));

            // برای تبدیل در جهت معکوس (DTO -> Entity) — مفید برای create/update
            CreateMap<CusProDto, Customer>()
                // اگر Customer دارای collection/navigation است، آن‌ها را ایگنور کن
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.addressDto))
                .ForMember(dest => dest.Orders, opt => opt.Ignore())   // مثال: collection ها را ایگنور کن
                .ForMember(dest => dest.products, opt => opt.Ignore()) // بر اساس مدلت
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()); // CreateDate معمولاً سمت سرور set می‌شود

            // Entity -> ReadDto
            CreateMap<Product, ProductDto>();
            // CreateDto -> Entity
            CreateMap<ProductDto, Product>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.DateOfOperation, o => o.Ignore())
                .ForMember(d => d.Orders, o => o.Ignore())
                .ForMember(d => d.customers, o => o.Ignore())
                .ForMember(d => d.sellers, o => o.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Shop -> ShopDto (خواندن)
            CreateMap<Shop, ShopDto>()
                .ForMember(d => d.NumberOfproducts, opt => opt.MapFrom(s => s.products != null ? s.products.Count : 0))
                .ForMember(d => d.ImagePath, opt => opt.MapFrom(s => string.IsNullOrEmpty(s.ImagePath) ? "/images/default-avatar.png" : s.ImagePath))
                .ForMember(d => d.DislikesCount, opt => opt.MapFrom(s => s.DislikesCount))
                .ForMember(d => d.AddressDto, opt => opt.MapFrom(s => s.Address));
            // ShopDto -> Shop (برای Create/Update)
            CreateMap<ShopDto, Shop>()
                // Id را فقط درصورتی که DTO شامل Id معتبر است نگاشت کن (تا در آپدیت Id موجود حفظ شود)
                .ForMember(d => d.Id, opt => opt.Condition((src, dest, srcMember) => srcMember != null && srcMember != Guid.Empty))
                .ForMember(d => d.Address, opt => opt.MapFrom(s => s.AddressDto))
                .ForMember(d => d.ImagePath, opt => opt.MapFrom(s => s.ImagePath))
                .ForMember(d => d.products, opt => opt.Ignore())
                .ForMember(d => d.LikesCount, opt => opt.Ignore())
                .ForMember(d => d.DislikesCount, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SliderImage, SliderImageDto>()
                .ForMember(d => d.ImagePath, opt => opt.MapFrom(s => string.IsNullOrEmpty(s.ImagePath) ? "/images/default-slider.jpg" : s.ImagePath)).ReverseMap();

        }
    }
}
