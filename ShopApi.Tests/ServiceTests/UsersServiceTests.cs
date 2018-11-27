using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using ShopApi.Data.Models;
using ShopApi.Logic.Dtos.Requests;
using ShopApi.Logic.Enums;
using ShopApi.Logic.Services;

namespace ShopApi.Tests.ServiceTests
{
    [TestFixture]
    public class UsersServiceTests
    {
        private AuthService _authService;
        private Mock<UserManager<User>> _userManager;
        private Mock<IConfiguration> _config;

        [SetUp]
        public void Setup()
        {
            _config = new Mock<IConfiguration>();
            _userManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            _authService = new AuthService(_config.Object, _userManager.Object);

            _config.Setup(c=>c["Secret"]).Returns("Random secret string");
        }

        [Test]
        public async Task LoginAsync_InvalidId_ReturnEmptyString()
        {
            var result = await _authService.LoginAsync(new LoginRequestDto());

            Assert.That(result, Is.EqualTo(string.Empty));
            _userManager.Verify(pr => pr.FindByNameAsync(null));
        }

        [Test]
        public async Task LoginAsync_ValidId_ReturnToken()
        {
            var login = new LoginRequestDto
            {
                UserName = "admin",
                Password = "admin"
            };
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                Role = Role.Admin.ToString()
            };

            _userManager.Setup(um => um.FindByNameAsync(login.UserName)).ReturnsAsync(user);
            _userManager.Setup(um => um.CheckPasswordAsync(user, login.Password)).ReturnsAsync(true);

            var result = await _authService.LoginAsync(login);

            Assert.That(result, Is.Not.EqualTo(string.Empty));
            _userManager.Verify(pr => pr.FindByNameAsync(login.UserName));
            _userManager.Verify(um => um.CheckPasswordAsync(user, login.Password));
        }

        //[Test]
        //public async Task GetAsync_ValidId_ReturnCategoryDto()
        //{
        //    var category = new Category();
        //    var categoryDto = new CategoryResponseDto();

        //    _userManager.Setup(pr => pr.GetAsync(It.IsAny<Guid>())).ReturnsAsync(category);
        //    _mapper.Setup(m => m.Map<Category, CategoryResponseDto>(category)).Returns(categoryDto);

        //    var result = await _authService.GetAsync(Guid.NewGuid());

        //    Assert.That(result, Is.EqualTo(categoryDto));
        //    _userManager.Verify(pr => pr.GetAsync(It.IsAny<Guid>()));
        //}
    }
}