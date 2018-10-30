using AutoMapper;
using ShopApi.Data.Models;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;

namespace ShopApi.Logic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductRequestDto, Product>();
            CreateMap<Product, ProductResponseDto>();

            CreateMap<CategoryRequestDto, Category>();
            CreateMap<Category, CategoryResponseDto>();

            CreateMap<CartItem, CartItemResponseDto>();
        }
    }
}