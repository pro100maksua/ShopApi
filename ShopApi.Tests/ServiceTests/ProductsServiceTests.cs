using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using ShopApi.Data.Interfaces;
using ShopApi.Data.Models;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;
using ShopApi.Logic.Services;

namespace ShopApi.Tests.ServiceTests
{
    [TestFixture]
    public class ProductsServiceTests
    {
        private ProductsService _productsService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IProductRepository> _productRepository;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Setup()
        {
            _productRepository = new Mock<IProductRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _productsService = new ProductsService(_unitOfWork.Object, _mapper.Object);
            _unitOfWork.Setup(uow => uow.ProductRepository).Returns(_productRepository.Object);
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnProductDtos()
        {
            var products = new List<Product>();
            var productDtos = new List<ProductResponseDto>();

            _productRepository
                .Setup(pr =>
                    pr.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(products);
            _mapper.Setup(m => m.Map<IEnumerable<Product>, IEnumerable<ProductResponseDto>>(products))
                .Returns(productDtos);

            var result = await _productsService.GetAllAsync(new FetchRequestDto());

            Assert.That(result, Is.EqualTo(productDtos));
            _productRepository.Verify(pr =>
                pr.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Product, bool>>>()));
        }

        [Test]
        public async Task GetAsync_InvalidId_ReturnNull()
        {
            var result = await _productsService.GetAsync(Guid.Empty);

            Assert.That(result, Is.Null);
            _productRepository.Verify(pr => pr.GetWithCategoryAsync(Guid.Empty));
        }

        [Test]
        public async Task GetAsync_ValidId_ReturnProductDto()
        {
            var product = new Product();
            var productDto = new ProductResponseDto();

            _productRepository.Setup(pr => pr.GetWithCategoryAsync(It.IsAny<Guid>())).ReturnsAsync(product);
            _mapper.Setup(m => m.Map<Product, ProductResponseDto>(product)).Returns(productDto);

            var result = await _productsService.GetAsync(Guid.NewGuid());

            Assert.That(result, Is.EqualTo(productDto));
            _productRepository.Verify(pr => pr.GetWithCategoryAsync(It.IsAny<Guid>()));
        }

        [Test]
        public async Task PostAsync_DuplicateName_ReturnErrors()
        {
            _productRepository.Setup(pr => pr.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(true);

            var result = await _productsService.PostAsync(new ProductRequestDto());

            Assert.That(result.Data, Is.Null);
            Assert.That(result.Errors, Is.Not.Empty);
            _productRepository.Verify(pr => pr.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task PostAsync_ValidObject_ReturnNewProduct()
        {
            var productToSave = new ProductRequestDto();
            var product = new Product();
            var productDto = new ProductResponseDto();

            _productRepository.Setup(pr => pr.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);
            _unitOfWork.Setup(uow => uow.CategoryRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new Category());
            _mapper.Setup(m => m.Map<ProductRequestDto, Product>(productToSave)).Returns(product);
            _mapper.Setup(m => m.Map<Product, ProductResponseDto>(product)).Returns(productDto);

            var result = await _productsService.PostAsync(productToSave);

            Assert.That(result.Data, Is.EqualTo(productDto));
            Assert.That(result.Errors, Is.Empty);
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
            _productRepository.Verify(pr => pr.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()));
            _productRepository.Verify(pr => pr.AddAsync(product));
        }

        [Test]
        public async Task PutAsync_InvalidId_ReturnNull()
        {
            var result = await _productsService.PutAsync(Guid.Empty, null);

            _productRepository.Verify(pr => pr.GetAsync(Guid.Empty));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PutAsync_DuplicateName_ReturnErrors()
        {
            var productToSave = new ProductRequestDto();
            var product = new Product();

            _productRepository.Setup(pr => pr.GetAsync(It.IsAny<Guid>())).ReturnsAsync(product);

            var result = await _productsService.PutAsync(Guid.NewGuid(), productToSave);

            Assert.That(result.Data, Is.Null);
            Assert.That(result.Errors, Is.Not.Empty);
            _productRepository.Verify(pr => pr.GetAsync(It.IsAny<Guid>()));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task PutAsync_ValidObject_ReturnUpdatedProduct()
        {
            var productToSave = new ProductRequestDto { Name = "NewName" };
            var product = new Product();
            var productDto = new ProductResponseDto();

            _productRepository.Setup(pr => pr.GetAsync(It.IsAny<Guid>())).ReturnsAsync(product);
            _unitOfWork.Setup(uow => uow.CategoryRepository.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new Category());
            _mapper.Setup(m => m.Map<ProductRequestDto, Product>(productToSave)).Returns(product);
            _mapper.Setup(m => m.Map<Product, ProductResponseDto>(product)).Returns(productDto);

            var result = await _productsService.PutAsync(Guid.NewGuid(), productToSave);

            Assert.That(result.Data, Is.EqualTo(productDto));
            _productRepository.Verify(pr => pr.GetAsync(It.IsAny<Guid>()));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_InvalidId_Return()
        {
            _productRepository.Setup(pr => pr.Remove(It.IsAny<Guid>())).Returns(false);

            var result = await _productsService.DeleteAsync(Guid.Empty);

            Assert.That(result, Is.EqualTo(false));
            _productRepository.Verify(pr => pr.Remove(Guid.Empty));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_ValidId_DeletesProduct()
        {
            _productRepository.Setup(pr => pr.Remove(It.IsAny<Guid>())).Returns(true);

            var result = await _productsService.DeleteAsync(Guid.NewGuid());

            Assert.That(result, Is.EqualTo(true));
            _productRepository.Verify(pr => pr.Remove(It.IsAny<Guid>()));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }
    }
}