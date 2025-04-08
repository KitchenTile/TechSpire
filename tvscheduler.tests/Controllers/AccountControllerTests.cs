using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using tvscheduler.Controllers;
using tvscheduler.Models;
using Xunit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using tvscheduler.Utilities;

namespace tvscheduler.tests.Controllers
{
    public class TestRecommendationGeneratorIndividual : RecommendationGeneratorIndividual
    {
        private readonly Show? _mockShow;

        public TestRecommendationGeneratorIndividual(Show mockShow, AppDbContext dbContext, TodaysShowsCache todaysShowsCache) 
            : base(dbContext, todaysShowsCache)
        {
            _mockShow = mockShow;
        }

        public new Task<Show?> SetIndividualRecommendation(string userId)
        {
            return Task.FromResult(_mockShow);
        }
    }

    public class AccountControllerTests : TestBase
    {
        private readonly AccountController _controller;
        private readonly Mock<UserManager<User>> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly string _testUserId = "test-user-123";
        private readonly TestRecommendationGeneratorIndividual _recommendationGenerator;

        public AccountControllerTests()
        {
            // Setup for all tests using base class methods
            _userManager = CreateMockUserManager();
            _dbContext = CreateInMemoryDbContext();
            
            // Use TestDataFactory to create mock show
            var mockShow = TestDataFactory.CreateShow();
            _recommendationGenerator = new TestRecommendationGeneratorIndividual(mockShow, _dbContext, new TodaysShowsCache());
            
            // Seed data needed for schedule tests
            TestDataFactory.SeedBasicTestData(_dbContext);
            
            // Create controller
            _controller = new AccountController(
                _dbContext,
                _userManager.Object,
                CreateMockConfiguration().Object,
                _recommendationGenerator
            );
        }
        
        // Helper method to simulate authenticated user for tests
        private void SetupAuthenticatedUser()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId),
                new Claim(ClaimTypes.Name, "testuser")
            };
            
            var identity = new ClaimsIdentity(claims, "Test");
            var user = new ClaimsPrincipal(identity);
            
            var httpContext = new DefaultHttpContext
            {
                User = user
            };
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext // Set the controller context to the authenticated user
            };
        }

        [Fact]
        public async Task Registration_WithNewUser_ReturnsOkResult()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Name = "newuser",
                Password = "Password123!"
            };
            
            _userManager.Setup(x => x.FindByNameAsync(loginDto.Name))
                .ReturnsAsync((User)null); // User doesn't exist
            
            // Act
            var result = await _controller.Registration(loginDto);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode); // 200 OK
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
            
            _userManager.Setup(x => x.FindByNameAsync(loginDto.Name))
                .ReturnsAsync(new User { UserName = loginDto.Name }); // User exists
            
            // Act
            var result = await _controller.Registration(loginDto);
            
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode); // 400 Bad Request
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
                Email = "valid@example.com"
            };
            
            _userManager.Setup(x => x.FindByNameAsync(loginDto.Name))
                .ReturnsAsync(user);
                
            _userManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(true);
            
            await _recommendationGenerator.SetIndividualRecommendation(_testUserId);
            
            // Act
            var result = await _controller.Login(loginDto);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value); // Token is returned and !null
        }
        
        [Fact]
        public async Task AddShowToSchedule_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new AddShowToScheduleRequest { ShowEventId = 1 };
            SetupAuthenticatedUser();
            
            // Act
            var result = await _controller.AddShowToSchedule(request);
            
            // Assert
            Assert.IsType<OkResult>(result); // 200 OK
            
            var scheduledEvent = await _dbContext.ScheduleEvents
                .FirstOrDefaultAsync(e => e.UserId == _testUserId && e.ShowEventId == request.ShowEventId);
            Assert.NotNull(scheduledEvent); // verifies that event is added to the database
        }
        
        [Fact]
        public async Task RemoveShowFromSchedule_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var showEventId = 1;
            
            // Ensure event is in schedule before trying to remove it
            if (!await _dbContext.ScheduleEvents.AnyAsync(e => e.UserId == _testUserId && e.ShowEventId == showEventId))
            {
                _dbContext.ScheduleEvents.Add(new UserScheduleEvent
                {
                    UserId = _testUserId,
                    ShowEventId = showEventId
                });
                await _dbContext.SaveChangesAsync();
            }
            
            SetupAuthenticatedUser();
            
            var request = new AddShowToScheduleRequest { ShowEventId = showEventId };
            
            // Act
            var result = await _controller.RemoveShowFromSchedule(request);
            
            // Assert
            Assert.IsType<OkObjectResult>(result); // 200 OK
        }
    }
}