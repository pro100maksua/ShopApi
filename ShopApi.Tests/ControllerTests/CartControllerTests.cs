using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ShopApi.Controllers;
using ShopApi.Logic.Dtos.Responses;
using ShopApi.Logic.Interfaces;

namespace ShopApi.Tests.ControllerTests
{
    [TestFixture]
    public class CartControllerTests
    {
        private CartController _cartController;
        private Mock<ICartService> _cartService;

        [SetUp]
        public void SetUp()
        {
            _cartService = new Mock<ICartService>();
            _cartController = new CartController(_cartService.Object);
        }

        [Test]
        public async Task GetCartAsync_WhenCalled_ReturnCart()
        {
            var cart = new CartResponseDto();
            _cartService.Setup(cs => cs.GetCartAsync(It.IsAny<Guid>())).ReturnsAsync(cart);

            var result = await _cartController.GetCartAsync();

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.That(result.Value, Is.EqualTo(cart));
            _cartService.Verify(cs => cs.GetCartAsync(It.IsAny<Guid>()));
        }

        [Test]
        public async Task DeleteCartAsync_WhenCalled_DeleteCart()
        {
            var result = await _cartController.DeleteCartAsync();

            Assert.IsInstanceOf<OkResult>(result);
            _cartService.Verify(cs => cs.DeleteCartAsync(It.IsAny<Guid>()));
        }

        [Test]
        public async Task AddToCartAsync_InvalidId_ReturnNotFound()
        {
            _cartService.Setup(cs => cs.AddToCartAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _cartController.AddToCartAsync(Guid.Empty);

            Assert.IsInstanceOf<NotFoundResult>(result);
            _cartService.Verify(cs => cs.AddToCartAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));
        }

        [Test]
        public async Task AddToCartAsync_ValidId_ReturnOk()
        {
            _cartService.Setup(cs => cs.AddToCartAsync(It.IsAny<Guid>(),It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _cartController.AddToCartAsync(Guid.NewGuid());

            Assert.IsInstanceOf<OkResult>(result);
            _cartService.Verify(cs => cs.AddToCartAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));
        }

        [Test]
        public async Task RemoveFromCartAsync_InvalidId_ReturnNotFound()
        {
            _cartService.Setup(cs => cs.RemoveFromCartAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _cartController.RemoveFromCartAsync(Guid.Empty);

            Assert.IsInstanceOf<NotFoundResult>(result);
            _cartService.Verify(cs => cs.RemoveFromCartAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));
        }

        [Test]
        public async Task RemoveFromCartAsync_ValidId_ReturnOk()
        {
            _cartService.Setup(cs => cs.RemoveFromCartAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _cartController.RemoveFromCartAsync(Guid.NewGuid());

            Assert.IsInstanceOf<OkResult>(result);
            _cartService.Verify(cs => cs.RemoveFromCartAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));
        }
    }
}