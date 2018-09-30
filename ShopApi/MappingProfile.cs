using AutoMapper;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductRequestDto, Product>();
            CreateMap<Product, ProductResponseDto>();

            CreateMap<CategoryRequestDto, Category>();
            CreateMap<Category, CategoryResponseDto>();
        }
    }
}