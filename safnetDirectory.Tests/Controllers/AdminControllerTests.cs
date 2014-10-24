using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using safnetDirectory.FullMvc.Controllers;
using safnetDirectory.FullMvc.Models;
using System.Web.Mvc;

namespace safnetDirectory.FullMvc.Tests.Controllers
{
    /// <summary>
    /// Summary description for AdminControllerTests
    /// </summary>
    [TestClass]
    public class AdminControllerTests
    {

        private MockRepository _repo;
        private Mock<IApplicationSignInManager> _mockSignInManager;
        private Mock<IApplicationUserManager> _mockUserManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _repo = new MockRepository(MockBehavior.Strict);
            _mockSignInManager = _repo.Create<IApplicationSignInManager>();
            _mockUserManager = _repo.Create<IApplicationUserManager>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _repo.VerifyAll();
        }

        private AdminController GivenTheSystemUnderTest()
        {
            return new AdminController(_mockUserManager.Object, _mockSignInManager.Object);
        }

        [TestMethod]
        public void PostValidRegistrationInformationAndUserSuccessfullyCreated()
        {
            var email = "president@whitehouse.gov";
            var password = "IAmNumber44!";
            var officeNumber = "(555) 123-2345";
            var mobileNumber = "(555) 867-5309";
            var title = "Worker bee";
            var location = "Atlantis";
            var fullName = "Catfish Hunter";
            var viewModel = GivenRegistrationInformation(email, password, fullName, title, location, officeNumber, mobileNumber, false);

            ExpectToCreateAuser(email, password, true, fullName, title, location, officeNumber, mobileNumber);
            ExpectToAssignToUserRole(true);
            
            var system = GivenTheSystemUnderTest();
            var result = WhenIRegisterAsANewUser(viewModel, system);

            ThenIWillBeRedirectedBackToHome(result);
        }


        [TestMethod]
        public void PostValidHruserRegistrationInformationAndUserSuccessfullyCreated()
        {
            var email = "president@whitehouse.gov";
            var password = "IAmNumber44!";
            var officeNumber = "(555) 123-2345";
            var mobileNumber = "(555) 867-5309";
            var title = "Worker bee";
            var location = "Atlantis";
            var fullName = "Catfish Hunter";
            var viewModel = GivenRegistrationInformation(email, password, fullName, title, location, officeNumber, mobileNumber, true);

            ExpectToCreateAuser(email, password, true, fullName, title, location, officeNumber, mobileNumber);
            ExpectToAssignToHrRole(true);
            ExpectToAssignToUserRole(true);

            var system = GivenTheSystemUnderTest();
            var result = WhenIRegisterAsANewUser(viewModel, system);

            ThenIWillBeRedirectedBackToHome(result);
        }

        private void ExpectToAssignToHrRole(bool success)
        {
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<string>(), It.Is<string>(y => y == AdminController.HR_ROLE)))
                .ReturnsAsync(new IdentityResultTss(success) as IdentityResult);
        }

        private void ExpectToAssignToUserRole(bool success)
        {
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<string>(), It.Is<string>(y => y == AdminController.USER_ROLE)))
                .ReturnsAsync(new IdentityResultTss(success) as IdentityResult);
        }

        private static void ThenIWillBeRedirectedBackToHome(ActionResult result)
        {
            var redirect = result as RedirectToRouteResult;
            Assert.IsNotNull(redirect, "wrong result type");

            Assert.IsTrue(redirect.RouteValues.ContainsKey("action"), "action key");
            Assert.AreEqual("Index", redirect.RouteValues["action"], "action value");
            Assert.IsTrue(redirect.RouteValues.ContainsKey("controller"), "controller key");
            Assert.AreEqual("Home", redirect.RouteValues["controller"], "controller value");
        }

        private static ActionResult WhenIRegisterAsANewUser(RegisterViewModel viewModel, AdminController system)
        {
            var task = system.Register(viewModel);
            task.Wait();

            var result = task.Result;
            return result;
        }

        private void ExpectToCreateAuser(string email, string password, bool success, string fullName, string title, string location, string officeNumber, string mobileNumber)
        {
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback((ApplicationUser actualUser, string actualPassword) =>
                {
                    Assert.AreEqual(email, actualUser.UserName, "UserName");
                    Assert.AreEqual(email, actualUser.Email, "Email");
                    Assert.AreEqual(password, actualPassword, "Password");
                    Assert.AreEqual(fullName, actualUser.FullName, "FullName");
                    Assert.AreEqual(title, actualUser.Title, "Title");
                    Assert.AreEqual(location, actualUser.Location, "Location");
                    Assert.AreEqual(officeNumber, actualUser.PhoneNumber, "PhoneNumber");
                    Assert.AreEqual(mobileNumber, actualUser.MobilePhoneNumber, "MobilePhoneNumber");
                })
                .ReturnsAsync(new IdentityResultTss(success) as IdentityResult);
        }

        private void ExpectToAutomateSignInOfuser()
        {
            _mockSignInManager.Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.Is<bool>(y => y == false), It.Is<bool>(y => y == false)))
                .Callback((ApplicationUser actualUser, bool isPersistent, bool rememberBrowser) =>
                {
                    Assert.IsFalse(string.IsNullOrWhiteSpace(actualUser.Id), "SignIn Id");
                })
                .Returns(() =>
                {
                    var task = new System.Threading.Tasks.Task(() => { return; });
                    task.Start();
                    return task;
                });
        }


        private RegisterViewModel GivenRegistrationInformation(string email, string password, string fullName, string title, string location, string officeNumber, string mobileNumber, bool isHruser)
        {
            return new RegisterViewModel
            {
                Email = email,
                Password = password,
                FullName = fullName,
                Title = title,
                Location = location,
                OfficePhoneNumber = officeNumber,
                MobilePhoneNumber = mobileNumber,
                IsHrUser = isHruser
            };
        }
    }

    public class IdentityResultTss : IdentityResult
    {
        public IdentityResultTss(bool success) : base(success) { }
    }
}
