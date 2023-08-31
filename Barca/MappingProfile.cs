using AutoMapper;
using Barca.DTOs;
using Barca.Entities;

namespace Barca
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category
            CreateMap<Category, CategoryDTO>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
            CreateMap<CategoryDTO, Category>();


            // Brand
            CreateMap<Brand, BrandDTO>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
            CreateMap<BrandDTO, Brand>();
            /*
            ANH XA TU BRAND SANG BRANDDTO
            ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products)): Đây là cách bạn đang chỉ định cách ánh xạ dữ liệu từ thuộc tính Products của đối tượng Brand sang thuộc tính Products của đối tượng BrandDTO.

                dest.Products: Đây là thuộc tính Products trong đối tượng BrandDTO.
                src.Products: Đây là thuộc tính Products trong đối tượng Brand.
            */


            // FootballClub
            CreateMap<FootballClub, FootballClubDTO>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
            CreateMap<FootballClubDTO, FootballClub>();


            // Product
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null))
                .ForMember(dest => dest.FootballClubName, opt => opt.MapFrom(src => src.Club.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts))
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages));
            CreateMap<ProductDTO, Product>();


            // ProductVariant
            CreateMap<ProductVariant, ProductVariantDTO>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product != null ? src.Product : null))
                .ForMember(dest => dest.MatchKind, opt => opt.MapFrom(src => src.MatchKind != null ? src.MatchKind : null))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size != null ? src.Size : null));
            CreateMap<ProductVariantDTO, ProductVariant>();


            // ProductImage
            CreateMap<ProductImage, ProductImageDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.MatchKindName, opt => opt.MapFrom(src => src.MatchKind != null ? src.MatchKind.MatchKindName : null));
            CreateMap<ProductImageDTO, ProductImage>();


            // Admin
            CreateMap<Admin, AdminDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User != null ? src.User : null));
            CreateMap<AdminDTO, Admin>();


            // UserAddress
            CreateMap<UserAddress, UserAddressDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));
            CreateMap<UserAddressDTO, UserAddress>();


            // Match Kind
            CreateMap<MatchKind, MatchKindDTO>()
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages));
            CreateMap<MatchKindDTO, MatchKind>();


            // UserDiscount
            CreateMap<UserDiscount, UserDiscountDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount));
            CreateMap<UserDiscountDTO, UserDiscount>();

            CreateMap<DiscountCode, DiscountCodeDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Size, SizeDTO>().ReverseMap();




            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<OrderProduct, OrderProductDTO>().ReverseMap();
        }
    }
}
