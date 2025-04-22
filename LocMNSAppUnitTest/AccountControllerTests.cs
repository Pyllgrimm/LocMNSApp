using LocMNSApp.Controllers;
using LocMNSApp.DTOs;
using LocMNSApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace LocMNSApp.Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<SignInManager<Utilisateur>> _signInManagerMock;
        private Mock<UserManager<Utilisateur>> _userManagerMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IAuthenticationService> _authenticationServiceMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private Mock<ITempDataDictionaryFactory> _tempDataDictionaryFactoryMock;
        private AccountController _controller;

        [TestInitialize]
        public void Setup()
        {
            _userManagerMock = MockUserManager();
            _signInManagerMock = MockSignInManager(_userManagerMock.Object);
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _tempDataDictionaryFactoryMock = new Mock<ITempDataDictionaryFactory>();

            var tempDataDictionaryMock = new Mock<ITempDataDictionary>();
            _tempDataDictionaryFactoryMock.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(tempDataDictionaryMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new ServiceCollection()
                .AddSingleton(_authenticationServiceMock.Object)
                .AddSingleton(_tempDataDictionaryFactoryMock.Object)
                .BuildServiceProvider();

            _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(httpContext);

            _controller = new AccountController(_signInManagerMock.Object, _userManagerMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _controller.Url = _urlHelperMock.Object;
            _controller.TempData = tempDataDictionaryMock.Object;
        }

        private Mock<UserManager<Utilisateur>> MockUserManager()
        {
            var store = new Mock<IUserStore<Utilisateur>>();
            var userManager = new Mock<UserManager<Utilisateur>>(
                store.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<Utilisateur>>().Object,
                new IUserValidator<Utilisateur>[0],
                new IPasswordValidator<Utilisateur>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<Utilisateur>>>().Object
            );
            return userManager;
        }

        private Mock<SignInManager<Utilisateur>> MockSignInManager(UserManager<Utilisateur> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<Utilisateur>>();
            var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<Utilisateur>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<Utilisateur>>();

            return new Mock<SignInManager<Utilisateur>>(
                userManager,
                contextAccessor.Object,
                claimsFactory.Object,
                optionsAccessor.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object
            );
        }

        [TestMethod]
        public void Login_ReturnsViewResult()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Login_Post_InvalidModelState_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Username", "Required");
            var model = new LoginDto();

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.IsNotNull(result);
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
        }

        [TestMethod]
        public async Task Login_Post_ValidModelState_RedirectsToHomeIndex()
        {
            // Arrange
            var model = new LoginDto { Username = "test", Password = "password", SeSouvenirDeMoi = true };
            _signInManagerMock.Setup(s => s.PasswordSignInAsync(model.Username, model.Password, model.SeSouvenirDeMoi, false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _authenticationServiceMock.Setup(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.IsNotNull(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("Home", redirectToActionResult.ControllerName);
        }

        [TestMethod]
        public void Register_ReturnsViewResult()
        {
            // Act
            var result = _controller.Register();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Register_Post_InvalidModelState_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");
            var model = new RegisterDto();

            // Act
            var result = await _controller.Register(model);

            // Assert
            Assert.IsNotNull(result);
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
        }



        [TestMethod]
        public async Task Logout_RedirectsToLogin()
        {
            // Act
            var result = await _controller.Logout();

            // Assert
            _signInManagerMock.Verify(sm => sm.SignOutAsync(), Times.Once);
            Assert.IsNotNull(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Login", redirectToActionResult.ActionName);
            Assert.AreEqual("Account", redirectToActionResult.ControllerName);
        }
    }
}
