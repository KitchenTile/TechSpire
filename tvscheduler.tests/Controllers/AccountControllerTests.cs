using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using tvscheduler.Controllers;
using tvscheduler.Models;
using tvscheduler.tests.Helpers;
using Xunit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace tvscheduler.tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AppDbContext _dbContext;
        private readonly AccountController _controller;
        private readonly string _testUserId = "testUser123";

        public AccountControllerTests()
        {
            // Setup UserManager mock
            _mockUserManager = MockHelpers.MockUserManager<User>();
            
            _mockConfiguration = new Mock<IConfiguration>(); // Initializing the IConfiguration mock
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("your-test-secret-key-with-at-least-32-characters-length");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("test-issuer");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("test-audience");
            
            // Setup database context
            _dbContext = TestDbContextFactory.CreateDbContext();
            
            // Create controller instance
            _controller = new AccountController(_dbContext, _mockUserManager.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Registration_WithNewUser_ReturnsOkResult()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Name = "newuser",  // "Name" in LoginDTO
                Password = "Password123!"
            };
            
            // Setup user manager to simulate successful creation
            _mockUserManager.Setup(x => x.FindByNameAsync(loginDto.Name))
                .ReturnsAsync((User)null); // fake user does not exist
                
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), loginDto.Password))
                .ReturnsAsync(IdentityResult.Success); // fake successful creation
            
            // Act
            var result = await _controller.Registration(loginDto);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Registration_WithExistingUsername_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Name = "existinguser",
                Password = "Password123!"
            };
            
            // Setup user manager to simulate existing user
            _mockUserManager.Setup(x => x.FindByNameAsync(loginDto.Name))
                .ReturnsAsync(new User { UserName = loginDto.Name });
            
            // Act
            var result = await _controller.Registration(loginDto);
            
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Name = "validuser",
                Password = "Password123!"
            };
            
            var user = new User
            {
                Id = _testUserId,
                UserName = loginDto.Name,
                Email = "validuser@example.com"
            };
            
            // Setup user manager to simulate successful login
            _mockUserManager.Setup(x => x.FindByNameAsync(loginDto.Name))
                .ReturnsAsync(user); // user exists
                
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(true); // password ok
            
            // Act
            var result = await _controller.Login(loginDto);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
        
        [Fact]
        public async Task AddShowToSchedule_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var showEventId = 1;
            var request = new AddShowToScheduleRequest { ShowEventId = showEventId };
            
            // Setup HttpContext with user claims
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId), //  to simulate an authenticated userID
                new Claim(ClaimTypes.Name, "testuser") // to simulate an authenticated username
            }, "mock")); // set aunth type to mock 
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext 
            };
            
            // Act
            var result = await _controller.AddShowToSchedule(request);
            
            // Assert
            var okResult = Assert.IsType<OkResult>(result); // Asserting that the result is of type OkResult.
            Assert.Equal(200, okResult.StatusCode);
        }
        
        [Fact]
        public async Task RemoveShowFromSchedule_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            // Add schedule event entry to remove
            var showEventId = 1;
            
            if (!_dbContext.ScheduleEvents.Any(use => use.UserId == _testUserId && use.ShowEventId == showEventId))
            {
                _dbContext.ScheduleEvents.Add(new UserScheduleEvent
                {
                    UserId = _testUserId,
                    ShowEventId = showEventId
                });
                _dbContext.SaveChanges();
            }
            
            // Setup HttpContext with user claims
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId),
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            var request = new AddShowToScheduleRequest { ShowEventId = showEventId };
            
            // Act
            var result = await _controller.RemoveShowFromSchedule(request);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value); // Asserting that the value of the result is not null.
        }
    }
}