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
            // AddressDto -> Address (برای Create/Update)
            CreateMap<AddressDto, Address>()
                // اگر DTO Id نداشته باشه یک Id جدید می‌سازیم (برای create-friendly)
                .ForMember(d => d.Id,
                    opt => opt.MapFrom(src => (src.Id.HasValue && src.Id.Value != Guid.Empty) ? src.Id.Value : Guid.NewGuid()))
                .ForMember(d => d.Customers, opt => opt.Ignore());

            // Address -> AddressDto (خواندن)
            CreateMap<Address, AddressDto>();

            // Customer -> CusProDto (نمایش)
            CreateMap<Customer, CusProDto>()
                .ForMember(dest => dest.addressDto, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Family, opt => opt.MapFrom(src => src.Family))
                .ForMember(dest => dest.NationalCode, opt => opt.MapFrom(src => src.NationalCode))
                .ForMember(dest => dest.Birth, opt => opt.MapFrom(src => src.Birth))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate));

            // CusProDto -> Customer (Create/Update)
            CreateMap<CusProDto, Customer>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.addressDto))
               // .ForMember(dest => dest.Id , opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore()) // ✅ در به‌روزرسانی تغییر نده
                .ForMember(dest => dest.Id, opt => opt.Condition((src, dest, srcMember) => src.Id != null && src.Id != Guid.Empty))
                .ForMember(dest => dest.Sales, opt => opt.Ignore())
                .ForMember(dest => dest.products, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => 
                              srcMember != null && !string.IsNullOrWhiteSpace(srcMember?.ToString())));


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
