using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using ShopApi.Data.Interfaces;
using ShopApi.Data.Models;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;
using ShopApi.Logic.Interfaces;

namespace ShopApi.Logic.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(FetchRequest request)
        {
            var isSearchEmpty = string.IsNullOrWhiteSpace(request.SearchString);
            Expression<Func<Product, bool>> filter = c => isSearchEmpty || c.Name.Contains(request.SearchString);

            var products = await _unitOfWork.ProductRepository.GetAllAsync(request.Skip, request.Take, filter);

            var productDtos = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductResponseDto>>(products);

            return productDtos;
        }

        public async Task<ProductResponseDto> GetAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetWithCategoryAsync(id);
            if (product == null)
            {
                return null;
            }

            var responseDto = _mapper.Map<Product, ProductResponseDto>(product);

            return responseDto;
        }

        public async Task<ProductResponseDto> PostAsync(ProductRequestDto requestDto)
        {
            var duplicate = await _unitOfWork.ProductRepository.FindAsync(p => p.Name == requestDto.Name);
            if (duplicate != null)
            {
                return null;
            }

            var product = _mapper.Map<ProductRequestDto, Product>(requestDto);
            product.Id = Guid.NewGuid();

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveAsync();

            product.Category = await _unitOfWork.CategoryRepository.GetAsync(product.CategoryId);
            var responseDto = _mapper.Map<Product, ProductResponseDto>(product);

            return responseDto;
        }

        public async Task<ProductResponseDto> PutAsync(Guid id, ProductRequestDto requestDto)
        {
            var productFromDb = await _unitOfWork.ProductRepository.GetAsync(id);
            if (productFromDb == null || productFromDb.Name == requestDto.Name)
            {
                return null;
            }

            _mapper.Map(requestDto, productFromDb);
            await _unitOfWork.SaveAsync();

            productFromDb.Category = await _unitOfWork.CategoryRepository.GetAsync(productFromDb.CategoryId);
            var responseDto = _mapper.Map<Product, ProductResponseDto>(productFromDb);

            return responseDto;
        }

        public async Task DeleteAsync(Guid id)
        {
            _unitOfWork.ProductRepository.Remove(id);

            await _unitOfWork.SaveAsync();
        }
    }
}
