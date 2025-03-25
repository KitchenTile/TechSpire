using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;

namespace tvscheduler.tests.Helpers
{
    public static class MockHelpers
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class // Method to create a mock UserManager
        {
            var store = new Mock<IUserStore<TUser>>(); // Creating a mock for IUserStore
            var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<TUser>>();
            var userValidators = new List<IUserValidator<TUser>>();
            var passwordValidators = new List<IPasswordValidator<TUser>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<TUser>>>();

            var mgr = new Mock<UserManager<TUser>>( // Creating a mock UserManager with the previously created mocks
                store.Object,
                optionsAccessor.Object,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                keyNormalizer.Object,
                errors.Object, 
                services.Object, 
                logger.Object);
            
            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success); // set up DeleteAsync
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success); // set up CreateAsync
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success); // set up UpdateAsync
            
            return mgr; // return the mock object UserManager
        }
    }
}