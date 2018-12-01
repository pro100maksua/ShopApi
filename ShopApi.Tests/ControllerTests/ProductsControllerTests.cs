using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ShopApi.Controllers;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Dtos.Responses;
using ShopApi.Logic.Interfaces;

namespace ShopApi.Tests.ControllerTests
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private ProductsController _productsController;
        private Mock<IProductsService> _productsService;

        [SetUp]
        public void SetUp()
        {
            _productsService = new Mock<IProductsService>();
            _productsController = new ProductsController(_productsService.Object);
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnProducts()
        {
            var productDtos = new FetchResult<ProductResponseDto>();
            _productsService.Setup(ps => ps.GetAllAsync(It.IsAny<FetchRequestDto>())).ReturnsAsync(productDtos);

            var result = await _productsController.GetAllAsync(new FetchRequestDto());

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.That(result.Value, Is.EqualTo(productDtos));
            _productsService.Verify(ps => ps.GetAllAsync(It.IsAny<FetchRequestDto>()));
        }

        [Test]
        public async Task GetAsync_InvalidId_ReturnNotFound()
        {
            var result = await _productsController.GetAsync(Guid.Empty);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
            _productsService.Verify(ps => ps.GetAsync(Guid.Empty));
        }

        [Test]
        public async Task GetAsync_ValidId_ReturnProduct()
        {
            var productDto = new ProductWithIncludeResponseDto();
            _productsService.Setup(ps => ps.GetAsync(It.IsAny<Guid>())).ReturnsAsync(productDto);

            var result = await _productsController.GetAsync(Guid.NewGuid());

            Assert.That(result.Value, Is.EqualTo(productDto));
            _productsService.Verify(ps => ps.GetAsync(It.IsAny<Guid>()));
        }

        [Test]
        public async Task PostAsync_DuplicateName_ReturnBadRequest()
        {
            var product = new Result<ProductWithIncludeResponseDto>();
            _productsService.Setup(ps => ps.PostAsync(It.IsAny<ProductRequestDto>())).ReturnsAsync(product);

            var result = await _productsController.PostAsync(new ProductRequestDto());

            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            _productsService.Verify(ps => ps.PostAsync(It.IsAny<ProductRequestDto>()));
        }

        [Test]
        public async Task PostAsync_WhenCalled_ReturnCreatedProduct()
        {
            var product = new Result<ProductWithIncludeResponseDto>();
            _productsService.Setup(ps => ps.PostAsync(It.IsAny<ProductRequestDto>())).ReturnsAsync(product);

            var result = await _productsController.PostAsync(new ProductRequestDto());

            Assert.That(result.Value, Is.EqualTo(product.Data));
            _productsService.Verify(ps => ps.PostAsync(It.IsAny<ProductRequestDto>()));
        }

        [Test]
        public async Task PutAsync_InvalidId_ReturnNotFound()
        {
            var result = await _productsController.PutAsync(Guid.Empty, new ProductRequestDto());

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
            _productsService.Verify(ps => ps.PutAsync(Guid.Empty, It.IsAny<ProductRequestDto>()));
        }

        [Test]
        public async Task PutAsync_DuplicateName_ReturnBadRequest()
        {
            _productsService.Setup(ps => ps.PutAsync(It.IsAny<Guid>(), It.IsAny<ProductRequestDto>()))
                .ReturnsAsync(new Result<ProductWithIncludeResponseDto>());

            var result = await _productsController.PutAsync(Guid.NewGuid(), new ProductRequestDto());

            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            _productsService.Verify(ps => ps.PutAsync(It.IsAny<Guid>(), It.IsAny<ProductRequestDto>()));
        }

        [Test]
        public async Task PutAsync_WhenCalled_ReturnUpdatedProduct()
        {
            var product = new Result<ProductWithIncludeResponseDto>();
            _productsService.Setup(ps => ps.PutAsync(It.IsAny<Guid>(), It.IsAny<ProductRequestDto>()))
                .ReturnsAsync(product);

            var result = await _productsController.PutAsync(Guid.NewGuid(), new ProductRequestDto());

            Assert.That(result.Value, Is.EqualTo(product.Data));
            _productsService.Verify(ps => ps.PutAsync(It.IsAny<Guid>(), It.IsAny<ProductRequestDto>()));
        }
        
        [Test]
        public async Task DeleteAsync_InvalidId_ReturnNotFound()
        {
            _productsService.Setup(ps => ps.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _productsController.DeleteAsync(Guid.Empty);

            Assert.IsInstanceOf<NotFoundResult>(result);
            _productsService.Verify(ps => ps.DeleteAsync(It.IsAny<Guid>()));
        }

        [Test]
        public async Task DeleteAsync_ValidId_ReturnOk()
        {
            _productsService.Setup(ps => ps.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _productsController.DeleteAsync(Guid.NewGuid());

            Assert.IsInstanceOf<OkResult>(result);
            _productsService.Verify(ps => ps.DeleteAsync(It.IsAny<Guid>()));
        }
    }
}