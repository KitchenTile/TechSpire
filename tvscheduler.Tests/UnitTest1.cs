using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Controllers;
using tvscheduler.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System;

namespace tvscheduler.Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<IConfiguration> _mockConfig;
        private Mock<AppDbContext> _mockDbContext;
        private AccountController _controller;

        [TestInitialize]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _mockConfig = new Mock<IConfiguration>();
            _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());

            _controller = new AccountController(_mockDbContext.Object, _mockUserManager.Object, _mockConfig.Object);
        }

        [TestMethod]
        public async Task Registration_UserAlreadyExists_ReturnsBadRequest()
        {
            var loginDTO = new LoginDTO { Name = "testuser", Password = "password123" };
            _mockUserManager.Setup(u => u.FindByNameAsync(loginDTO.Name)).ReturnsAsync(new User { UserName = "testuser" });

            var result = await _controller.Registration(loginDTO);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Registration_NewUser_ReturnsOk()
        {
            var loginDTO = new LoginDTO { Name = "newuser", Password = "password123" };
            _mockUserManager.Setup(u => u.FindByNameAsync(loginDTO.Name)).ReturnsAsync((User)null);
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), loginDTO.Password)).ReturnsAsync(IdentityResult.Success);

            var result = await _controller.Registration(loginDTO);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }

    [TestClass]
    public class TvControllerTests
    {
        private Mock<HttpClient> _mockHttpClient;
        private Mock<AppDbContext> _mockDbContext;
        private Mock<UserManager<User>> _mockUserManager;
        private TvController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _controller = new TvController(_mockHttpClient.Object, _mockDbContext.Object, _mockUserManager.Object);
        }

        [TestMethod]
        public async Task EntryEndpoint_UserNotAuthenticated_ReturnsUnauthorized()
        {
            var result = await _controller.EntryEndpoint();

            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }
    }
}