﻿using Microsoft.AspNet.Identity;

using Moq;

using NUnit.Framework;

using Ploeh.AutoFixture;

using RememBeer.Business.Services;
using RememBeer.Common.Identity.Contracts;
using RememBeer.Common.Identity.Models;
using RememBeer.Data.Repositories.Base;
using RememBeer.Models.Factories;
using RememBeer.Tests.Business.Mocks;
using RememBeer.Tests.Common;

namespace RememBeer.Tests.Business.Services.UserServiceTests
{
    [TestFixture]
    public class RegisterUser_Should : TestClassBase
    {
        [Test]
        public void CallFactoryCreateApplicationUserMethodOnce()
        {
            var userName = this.Fixture.Create<string>();
            var email = this.Fixture.Create<string>();
            var password = this.Fixture.Create<string>();
            var mockedUser = new MockedApplicationUser();

            var userManager = new Mock<IApplicationUserManager>();
            var signInManager = new Mock<IApplicationSignInManager>();
            var modelFactory = new Mock<IModelFactory>();
            modelFactory.Setup(f => f.CreateApplicationUser(userName, email))
                        .Returns(mockedUser);
            var userRepository = new Mock<IRepository<ApplicationUser>>();

            var service = new UserService(userManager.Object,
                                          signInManager.Object,
                                          userRepository.Object,
                                          modelFactory.Object);

            var result = service.RegisterUser(userName, email, password);

            modelFactory.Verify(f => f.CreateApplicationUser(userName, email), Times.Once);
        }

        [Test]
        public void CallUserManagerCreateMethodOnce()
        {
            var userName = this.Fixture.Create<string>();
            var email = this.Fixture.Create<string>();
            var password = this.Fixture.Create<string>();
            var mockedUser = new MockedApplicationUser();

            var userManager = new Mock<IApplicationUserManager>();
            var signInManager = new Mock<IApplicationSignInManager>();
            var modelFactory = new Mock<IModelFactory>();
            var userRepository = new Mock<IRepository<ApplicationUser>>();

            modelFactory.Setup(f => f.CreateApplicationUser(userName, email))
                        .Returns(mockedUser);

            var service = new UserService(userManager.Object,
                                          signInManager.Object,
                                          userRepository.Object,
                                          modelFactory.Object);

            var result = service.RegisterUser(userName, email, password);

            userManager.Verify(m => m.Create(mockedUser, password), Times.Once);
        }

        [Test]
        public void ReturnResultFromUserManager()
        {
            var userName = this.Fixture.Create<string>();
            var email = this.Fixture.Create<string>();
            var password = this.Fixture.Create<string>();
            var mockedUser = new MockedApplicationUser();
            var expectedResult = IdentityResult.Failed();

            var userManager = new Mock<IApplicationUserManager>();
            userManager.Setup(m => m.Create(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                       .Returns(expectedResult);

            var signInManager = new Mock<IApplicationSignInManager>();
            var modelFactory = new Mock<IModelFactory>();
            modelFactory.Setup(f => f.CreateApplicationUser(userName, email))
                        .Returns(mockedUser);
            var userRepository = new Mock<IRepository<ApplicationUser>>();

            var service = new UserService(userManager.Object,
                                          signInManager.Object,
                                          userRepository.Object,
                                          modelFactory.Object);

            var result = service.RegisterUser(userName, email, password);

            Assert.AreSame(expectedResult, result);
        }
    }
}