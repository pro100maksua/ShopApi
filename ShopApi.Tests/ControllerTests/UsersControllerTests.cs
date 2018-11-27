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
    public class UsersControllerTests
    {
        private AuthController _authController;
        private Mock<IAuthService> _usersService;

        [SetUp]
        public void SetUp()
        {
            _usersService = new Mock<IAuthService>();
            _authController = new AuthController(_usersService.Object);
        }

        [Test]
        public async Task LoginAsync_Invalid_ReturnNotFound()
        {
            var result = await _authController.LoginAsync(new LoginRequestDto());

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
            _usersService.Verify(ps => ps.LoginAsync(It.IsAny<LoginRequestDto>()));
        }

        [Test]
        public async Task LoginAsync_Valid_ReturnToken()
        {
            var token = "token";
            _usersService.Setup(ps => ps.LoginAsync(It.IsAny<LoginRequestDto>())).ReturnsAsync(token);

            var result = await _authController.LoginAsync(new LoginRequestDto());

            Assert.That(result.Value, Is.EqualTo(token));
            _usersService.Verify(ps => ps.LoginAsync(It.IsAny<LoginRequestDto>()));
        }
        [Test]
        public async Task RegisterAsync_Invalid_ReturnBadRequest()
        {
            var token = new Result<string>();
            _usersService.Setup(ps => ps.RegisterAsync(It.IsAny<RegisterRequestDto>())).ReturnsAsync(token);

            var result = await _authController.RegisterAsync(new RegisterRequestDto());

            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            _usersService.Verify(ps => ps.RegisterAsync(It.IsAny<RegisterRequestDto>()));
        }

        [Test]
        public async Task RegisterAsync_Valid_ReturnToken()
        {
            var token = new Result<string> {Data = "token"};
            _usersService.Setup(ps => ps.RegisterAsync(It.IsAny<RegisterRequestDto>())).ReturnsAsync(token);

            var result = await _authController.RegisterAsync(new RegisterRequestDto());

            Assert.That(result.Value, Is.EqualTo(token.Data));
            _usersService.Verify(ps => ps.RegisterAsync(It.IsAny<RegisterRequestDto>()));
        }
    }
}