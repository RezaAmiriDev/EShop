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
            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>()
                .ForMember(d => d.Id, opt => opt.Condition((src, dest, srcMember) =>
                src.Id.HasValue && src.Id.Value != Guid.Empty))
                .ForMember(d => d.Customers, opt => opt.Ignore());

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
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrWhiteSpace(srcMember?.ToString())));

            // Entity -> ReadDto
            CreateMap<Product, ProductDto>();
            // CreateDto -> Entity
            CreateMap<ProductDto, Product>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.DateOfOperation, o => o.Ignore())
                .ForMember(d => d.Sales, o => o.Ignore())
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

        }
    }
}
