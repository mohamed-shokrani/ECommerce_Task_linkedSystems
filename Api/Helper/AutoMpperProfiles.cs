using AutoMapper;
using Core.Dto;
using Core.Models;

namespace Api.Helper
{
    public class AutoMpperProfiles : Profile
    {
        public AutoMpperProfiles()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<ProductCreateDto, Product>();

        }
    }
}
