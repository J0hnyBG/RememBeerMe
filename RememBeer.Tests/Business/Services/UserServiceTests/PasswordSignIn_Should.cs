﻿using Microsoft.AspNet.Identity.Owin;

using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;

using RememBeer.Business.Services;
using RememBeer.Common.Identity.Contracts;
using RememBeer.Common.Identity.Models;
using RememBeer.Data.Repositories.Base;
using RememBeer.Models.Factories;
using RememBeer.Tests.Common;

namespace RememBeer.Tests.Business.Services.UserServiceTests
{
    [TestFixture]
    public class PasswordSignIn_Should : TestClassBase
    {
        [Test]
        public void CallSignInManagerPasswordSignInMethodOnce()
        {
            var email = this.Fixture.Create<string>();
            var password = this.Fixture.Create<string>();
            var isPersistent = this.Fixture.Create<bool>();

            var userManager = new Mock<IApplicationUserManager>();
            var signInManager = new Mock<IApplicationSignInManager>();
            var modelFactory = new Mock<IModelFactory>();
            var userRepository = new Mock<IRepository<ApplicationUser>>();

            var service = new UserService(userManager.Object,
                                          signInManager.Object,
                                          userRepository.Object,
                                          modelFactory.Object);

            var result = service.PasswordSignIn(email, password, isPersistent);

            signInManager.Verify(m => m.PasswordSignIn(email, password, isPersistent), Times.Once);
        }

        [TestCase(SignInStatus.Success)]
        [TestCase(SignInStatus.Failure)]
        [TestCase(SignInStatus.LockedOut)]
        [TestCase(SignInStatus.RequiresVerification)]
        public void ReturnResultFromSignInManagerPasswordSignIn(SignInStatus expectedStatus)
        {
            var email = this.Fixture.Create<string>();
            var password = this.Fixture.Create<string>();
            var isPersistent = this.Fixture.Create<bool>();

            var userRepository = new Mock<IRepository<ApplicationUser>>();
            var userManager = new Mock<IApplicationUserManager>();
            var modelFactory = new Mock<IModelFactory>();
            var signInManager = new Mock<IApplicationSignInManager>();
            signInManager.Setup(s => s.PasswordSignIn(email, password, isPersistent))
                         .Returns(expectedStatus);

            var service = new UserService(userManager.Object,
                                          signInManager.Object,
                                          userRepository.Object,
                                          modelFactory.Object);

            var result = service.PasswordSignIn(email, password, isPersistent);

            Assert.AreEqual(expectedStatus, result);
        }
    }
}