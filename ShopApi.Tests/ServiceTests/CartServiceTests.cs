using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using ShopApi.Data.Interfaces;
using ShopApi.Data.Models;
using ShopApi.Logic.Dtos.Responses;
using ShopApi.Logic.Services;

namespace ShopApi.Tests.ServiceTests
{
    [TestFixture]
    public class CartServiceTests
    {
        private Mock<ICartItemRepository> _cartItemRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mapper;
        private CartService _cartService;

        [SetUp]
        public void Setup()
        {
            _cartItemRepository = new Mock<ICartItemRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _cartService = new CartService(_unitOfWork.Object, _mapper.Object);
            _unitOfWork.Setup(uow => uow.CartItemRepository).Returns(_cartItemRepository.Object);
        }

        [Test]
        public async Task GetCartAsync_WhenCalled_ReturnUserCart()
        {
            var userId = Guid.NewGuid();
            var cartItems = new List<CartItem>();
            var cartItemDtos = new List<CartItemResponseDto>
            {
                new CartItemResponseDto {Count = 1, Product = new ProductResponseDto {Cost = 10}},
                new CartItemResponseDto {Count = 2, Product = new ProductResponseDto {Cost = 15}}
            };
            var expectedTotal = 40;

            _cartItemRepository.Setup(pr => pr.GetUserItemsAsync(userId)).ReturnsAsync(cartItems);
            _mapper.Setup(m => m.Map<IEnumerable<CartItem>, IEnumerable<CartItemResponseDto>>(cartItems))
                .Returns(cartItemDtos);

            var result = await _cartService.GetCartAsync(userId);

            Assert.That(result.Items, Is.EqualTo(cartItemDtos));
            Assert.That(result.Total, Is.EqualTo(expectedTotal));
            _cartItemRepository.Verify(pr => pr.GetUserItemsAsync(userId));
        }

        [Test]
        public async Task DeleteCartAsync_WhenCalled_DeleteUserCart()
        {
            var userId = Guid.NewGuid();
            var cartItems = new List<CartItem>();
            _cartItemRepository.Setup(pr => pr.GetUserItemsAsync(userId)).ReturnsAsync(cartItems);

            await _cartService.DeleteCartAsync(userId);

            _cartItemRepository.Verify(pr => pr.RemoveRange(cartItems));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task AddToCartAsync_InvalidProductId_Return()
        {
            _unitOfWork.Setup(ouw => ouw.ProductRepository.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);
            
            var result = await _cartService.AddToCartAsync(Guid.Empty, Guid.Empty);

            Assert.That(result, Is.False);
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task AddToCartAsync_NewProductAdded_AddNewCartItem()
        {
            _unitOfWork.Setup(ouw => ouw.ProductRepository.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(true);
            _cartItemRepository.Setup(pr => pr.FindAsync(It.IsAny<Expression<Func<CartItem, bool>>>()))
                .ReturnsAsync(default(CartItem));

            var result = await _cartService.AddToCartAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.That(result, Is.True);
            _cartItemRepository.Verify(pr => pr.AddAsync(It.IsAny<CartItem>()));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task AddToCartAsync_ProductAddedMoreThanOnce_IncrementCount()
        {
            var cartItem = new CartItem { Count = 1 };

            _unitOfWork.Setup(ouw => ouw.ProductRepository.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(true);
            _cartItemRepository.Setup(pr => pr.FindAsync(It.IsAny<Expression<Func<CartItem, bool>>>()))
                .ReturnsAsync(cartItem);

            var result = await _cartService.AddToCartAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.That(result, Is.True);
            Assert.That(cartItem.Count, Is.EqualTo(2));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task RemoveFromCartAsync_InvalidProductId_Return()
        {
            var result = await _cartService.RemoveFromCartAsync(Guid.Empty, Guid.Empty);

            Assert.That(result, Is.False);
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task RemoveFromCartAsync_OneProductInCart_RemoveFromRepo()
        {
            var cartItem = new CartItem { Count = 1 };
            _cartItemRepository.Setup(pr => pr.FindAsync(It.IsAny<Expression<Func<CartItem, bool>>>()))
                .ReturnsAsync(cartItem);

            var result = await _cartService.RemoveFromCartAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.That(result, Is.True);
            _cartItemRepository.Verify(cr => cr.Remove(cartItem));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task RemoveFromCartAsync_MoreThanOneProductInCart_DecrementCount()
        {
            var cartItem = new CartItem { Count = 2 };
            _cartItemRepository.Setup(pr => pr.FindAsync(It.IsAny<Expression<Func<CartItem, bool>>>()))
                .ReturnsAsync(cartItem);

            var result = await _cartService.RemoveFromCartAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.That(result, Is.True);
            Assert.That(cartItem.Count, Is.EqualTo(1));
            _unitOfWork.Verify(uow => uow.SaveAsync(), Times.Once);
        }
    }
}