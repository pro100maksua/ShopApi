using System;
using System.Collections.Generic;
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
    public class CategoriesControllerTests
    {
        private CategoriesController _categoriesController;
        private Mock<ICategoriesService> _categoriesService;

        [SetUp]
        public void SetUp()
        {
            _categoriesService = new Mock<ICategoriesService>();
            _categoriesController = new CategoriesController(_categoriesService.Object);
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnCategories()
        {
            IEnumerable<CategoryResponseDto> categoryDtos = new List<CategoryResponseDto>();
            _categoriesService.Setup(cs => cs.GetAllAsync(It.IsAny<FetchRequest>())).ReturnsAsync(categoryDtos);

            var result = await _categoriesController.GetAllAsync(new FetchRequest());

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.That((result as OkObjectResult)?.Value, Is.EqualTo(categoryDtos));
            _categoriesService.Verify(cs => cs.GetAllAsync(It.IsAny<FetchRequest>()));
        }

        [Test]
        public async Task GetAsync_InvalidId_ReturnNotFound()
        {
            var result = await _categoriesController.GetAsync(Guid.Empty);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
            _categoriesService.Verify(cs => cs.GetAsync(Guid.Empty));
        }

        [Test]
        public async Task GetAsync_ValidId_ReturnCategory()
        {
            var categoryDto = new CategoryResponseDto();
            _categoriesService.Setup(cs => cs.GetAsync(It.IsAny<Guid>())).ReturnsAsync(categoryDto);

            var result = await _categoriesController.GetAsync(Guid.NewGuid());

            Assert.That(result.Value, Is.EqualTo(categoryDto));
            _categoriesService.Verify(cs => cs.GetAsync(It.IsAny<Guid>()));
        }

        [Test]
        public async Task PostAsync_DuplicateName_ReturnBadRequest()
        {
            var category = new Result<CategoryResponseDto>();
            _categoriesService.Setup(cs => cs.PostAsync(It.IsAny<CategoryRequestDto>())).ReturnsAsync(category);

            var result = await _categoriesController.PostAsync(new CategoryRequestDto());

            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            _categoriesService.Verify(cs => cs.PostAsync(It.IsAny<CategoryRequestDto>()));
        }

        [Test]
        public async Task PostAsync_WhenCalled_ReturnCreatedCategory()
        {
            var category = new Result<CategoryResponseDto>();
            _categoriesService.Setup(cs => cs.PostAsync(It.IsAny<CategoryRequestDto>())).ReturnsAsync(category);

            var result = await _categoriesController.PostAsync(new CategoryRequestDto());

            Assert.That(result.Value, Is.EqualTo(category.Data));
            _categoriesService.Verify(cs => cs.PostAsync(It.IsAny<CategoryRequestDto>()));
        }

        [Test]
        public async Task PutAsync_InvalidId_ReturnNotFound()
        {
            var result = await _categoriesController.PutAsync(Guid.Empty, new CategoryRequestDto());

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
            _categoriesService.Verify(cs => cs.PutAsync(Guid.Empty, It.IsAny<CategoryRequestDto>()));
        }

        [Test]
        public async Task PutAsync_DuplicateName_ReturnBadRequest()
        {
            _categoriesService.Setup(cs => cs.PutAsync(It.IsAny<Guid>(), It.IsAny<CategoryRequestDto>()))
                .ReturnsAsync(new Result<CategoryResponseDto>());

            var result = await _categoriesController.PutAsync(Guid.NewGuid(), new CategoryRequestDto());

            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            _categoriesService.Verify(cs => cs.PutAsync(It.IsAny<Guid>(), It.IsAny<CategoryRequestDto>()));
        }

        [Test]
        public async Task PutAsync_WhenCalled_ReturnUpdatedCategory()
        {
            var category = new Result<CategoryResponseDto>();
            _categoriesService.Setup(cs => cs.PutAsync(It.IsAny<Guid>(), It.IsAny<CategoryRequestDto>()))
                .ReturnsAsync(category);

            var result = await _categoriesController.PutAsync(Guid.NewGuid(), new CategoryRequestDto());

            Assert.That(result.Value, Is.EqualTo(category.Data));
            _categoriesService.Verify(cs => cs.PutAsync(It.IsAny<Guid>(), It.IsAny<CategoryRequestDto>()));
        }

        [Test]
        public async Task DeleteAsync_InvalidId_ReturnNotFound()
        {
            _categoriesService.Setup(cs => cs.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _categoriesController.DeleteAsync(Guid.Empty);

            Assert.IsInstanceOf<NotFoundResult>(result);
            _categoriesService.Verify(cs => cs.DeleteAsync(It.IsAny<Guid>()));
        }

        [Test]
        public async Task DeleteAsync_ValidId_ReturnOk()
        {
            _categoriesService.Setup(cs => cs.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _categoriesController.DeleteAsync(Guid.NewGuid());

            Assert.IsInstanceOf<OkResult>(result);
            _categoriesService.Verify(cs => cs.DeleteAsync(It.IsAny<Guid>()));
        }
    }
}