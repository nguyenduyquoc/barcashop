using AutoMapper;
using Barca.DTOs;
using Barca.Entities;

namespace Barca
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Admin, AdminDTO>().ReverseMap();
            CreateMap<Brand, BrandDTO>().ReverseMap();
            CreateMap<DiscountCode, DiscountCodeDTO>().ReverseMap();
        }
    }
}
