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
                .ForMember(dest => dest.Sales, opt => opt.Ignore())   // مثال: collection ها را ایگنور کن
                .ForMember(dest => dest.products, opt => opt.Ignore()) // بر اساس مدلت
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()); // CreateDate معمولاً سمت سرور set می‌شود


            // Entity -> ReadDto
            CreateMap<Product, ProductReadDto>();

            // CreateDto -> Entity
            CreateMap<ProductCreateDto, Product>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.DateOfOperation, o => o.Ignore())
                .ForMember(d => d.ImagePath, o => o.Ignore())
                .ForMember(d => d.Sales, o => o.Ignore())
                .ForMember(d => d.customers, o => o.Ignore())
                .ForMember(d => d.sellers, o => o.Ignore());

            // UpdateDto -> Entity (map into existing), don't overwrite Id/DateOfOperation/ImagePath
            CreateMap<ProductUpdateDto, Product>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.DateOfOperation, o => o.Ignore())
                .ForMember(d => d.ImagePath, o => o.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Shop, SellerDto>()
     .ForMember(d => d.AddressId, opt => opt.MapFrom(s => s.Address))   // اگر AddressDto تنظیم است
     .ForMember(d => d.ProductsCount, opt => opt.Ignore()); // یا map محصولات طبق نیاز

            CreateMap<SellerDto, Shop>()
                .ForMember(d => d.products, opt => opt.Ignore())
                .ForMember(d => d.Id, opt => opt.Condition((src, dest, srcMember) => srcMember != null));


        }
    }
}
