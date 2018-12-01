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
    public class CategoriesServiceTests
    {
        private CategoriesService _categoriesService;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<ICategoryRepository> _categoryRepository;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Setup()
        {
            _categoryRepository = new Mock<ICategoryRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _categoriesService = new CategoriesService(_unitOfWork.Object, _mapper.Object);
            _unitOfWork.Setup(uow => uow.CategoryRepository).Returns(_categoryRepository.Object);
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnCategoryDtos()
        {
            var categories = new List<Category>();
            var categoryDtos = new List<CategoryResponseDto>();

            _categoryRepository
                .Setup(pr =>
                    pr.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(categories);
            _mapper.Setup(m => m.Map<IEnumerable<Category>, IEnumerable<CategoryResponseDto>>(categories))
                .Returns(categoryDtos);

            var result = await _categoriesService.GetAllAsync(new FetchRequestDto());

            Assert.That(result, Is.EqualTo(categoryDtos));
            _categoryRepository.Verify(pr =>
                pr.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Category, bool>>>()));
        }

        [Test]
        public async Task GetAsync_InvalidId_ReturnNull()
        {
            var result = await _categoriesService.GetAsync(Guid.Empty);

            Assert.That(result, Is.Null);
            _categoryRepository.Verify(pr => pr.GetAsync(Guid.Empty));
        }

        [Test]
        public async Task GetAsync_ValidId_ReturnCategoryDto()
        {
            var category = new Category();
            var categoryDto = new CategoryResponseDto();

            _categoryRepository.Setup(pr => pr.GetAsync(It.IsAny<Guid>())).ReturnsAsync(category);
            _mapper.Setup(m => m.Map<Category, CategoryResponseDto>(category)).Returns(categoryDto);

            var result = await _categoriesService.GetAsync(Guid.NewGuid());

            Assert.That(result, Is.EqualTo(categoryDto));
            _categoryRepository.Verify(pr => pr.GetAsync(It.IsAny<Guid>()));
        }

        [Test]
        public async Task PostAsync_DuplicateName_ReturnErrors()
        {
            _categoryRepository.Setup(pr => pr.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(true);

            var result = await _categoriesService.PostAsync(new CategoryRequestDto());

            Assert.That(result.Data, Is.Null);
            Assert.That(result.Errors, Is.Not.Empty);
            _categoryRepository.Verify(pr => pr.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>()));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task PostAsync_ValidObject_ReturnNewCategory()
        {
            var categoryToSave = new CategoryRequestDto();
            var category = new Category();
            var categoryDto = new CategoryResponseDto();

            _categoryRepository.Setup(pr => pr.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(false);
            _mapper.Setup(m => m.Map<CategoryRequestDto, Category>(categoryToSave)).Returns(category);
            _mapper.Setup(m => m.Map<Category, CategoryResponseDto>(category)).Returns(categoryDto);

            var result = await _categoriesService.PostAsync(categoryToSave);

            Assert.That(result.Data, Is.EqualTo(categoryDto));
            Assert.That(result.Errors, Is.Empty);
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
            _categoryRepository.Verify(pr => pr.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>()));
            _categoryRepository.Verify(pr => pr.AddAsync(category));
        }

        [Test]
        public async Task PutAsync_InvalidId_ReturnNull()
        {
            var result = await _categoriesService.PutAsync(Guid.Empty, null);

            _categoryRepository.Verify(pr => pr.GetAsync(Guid.Empty));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PutAsync_DuplicateName_ReturnErrors()
        {
            var categoryToSave = new CategoryRequestDto();
            var category = new Category();

            _categoryRepository.Setup(pr => pr.GetAsync(It.IsAny<Guid>())).ReturnsAsync(category);

            var result = await _categoriesService.PutAsync(Guid.NewGuid(), categoryToSave);

            Assert.That(result.Data, Is.Null);
            Assert.That(result.Errors, Is.Not.Empty);
            _categoryRepository.Verify(pr => pr.GetAsync(It.IsAny<Guid>()));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task PutAsync_ValidObject_ReturnUpdatedCategory()
        {
            var categoryToSave = new CategoryRequestDto { Name = "NewName" };
            var category = new Category();
            var categoryDto = new CategoryResponseDto();

            _categoryRepository.Setup(pr => pr.GetAsync(It.IsAny<Guid>())).ReturnsAsync(category);
            _mapper.Setup(m => m.Map<CategoryRequestDto, Category>(categoryToSave)).Returns(category);
            _mapper.Setup(m => m.Map<Category, CategoryResponseDto>(category)).Returns(categoryDto);

            var result = await _categoriesService.PutAsync(Guid.NewGuid(), categoryToSave);

            Assert.That(result.Data, Is.EqualTo(categoryDto));
            _categoryRepository.Verify(pr => pr.GetAsync(It.IsAny<Guid>()));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_InvalidId_Return()
        {
            _categoryRepository.Setup(pr => pr.Remove(It.IsAny<Guid>())).Returns(false);

            var result = await _categoriesService.DeleteAsync(Guid.Empty);

            Assert.That(result, Is.EqualTo(false));
            _categoryRepository.Verify(pr => pr.Remove(Guid.Empty));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_ValidId_DeletesCategory()
        {
            _categoryRepository.Setup(pr => pr.Remove(It.IsAny<Guid>())).Returns(true);

            var result = await _categoriesService.DeleteAsync(Guid.NewGuid());

            Assert.That(result, Is.EqualTo(true));
            _categoryRepository.Verify(pr => pr.Remove(It.IsAny<Guid>()));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }
    }
}