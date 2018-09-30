﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Services
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync(int skip, int take);
        Task<ProductResponseDto> GetAsync(string id);
        Task<ProductResponseDto> PostAsync(ProductRequestDto requestDto);
        Task<ProductResponseDto> PutProductAsync(string id, ProductRequestDto requestDto);
        Task<bool> DeleteAsync(string id);
    }

    public class ProductsService : IProductsService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductsService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(int skip, int take)
        {
            var products = await _context.Products.Skip(skip).Take(take).Include(p => p.Category).ToListAsync();

            var productDtos = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductResponseDto>>(products);

            return productDtos;
        }

        public async Task<ProductResponseDto> GetAsync(string id)
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.Category)
                .SingleOrDefaultAsync();

            if (product == null) return null;

            var responseDto = _mapper.Map<Product, ProductResponseDto>(product);

            return responseDto;
        }

        public async Task<ProductResponseDto> PostAsync(ProductRequestDto requestDto)
        {
            var product = _mapper.Map<ProductRequestDto, Product>(requestDto);
            product.Id = Guid.NewGuid().ToString();

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            product.Category = await _context.Categories.FindAsync(product.CategoryId);

            var responseDto = _mapper.Map<Product, ProductResponseDto>(product);

            return responseDto;
        }

        public async Task<ProductResponseDto> PutProductAsync(string id, ProductRequestDto requestDto)
        {
            var productFromDb = await _context.Products.FindAsync(id);

            if (productFromDb == null) return null;

            _mapper.Map(requestDto, productFromDb);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<Product, ProductResponseDto>(productFromDb);

            return responseDto;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return false;

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}