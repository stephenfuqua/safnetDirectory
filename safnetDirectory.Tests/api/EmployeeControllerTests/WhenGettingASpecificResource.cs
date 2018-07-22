using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using safnetDirectory.FullMvc.api;
using safnetDirectory.FullMvc.Data;
using safnetDirectory.FullMvc.Models;

namespace safnetDirectory.FullMvc.Tests.api.EmployeeControllerTests
{
    [TestFixture]
    public class WhenGettingASpecificResource
    {
        private EmployeeController _controller;
        private Mock<IDbContext> _mockDbContext;

        [SetUp]
        public void SetUp()
        {
            _mockDbContext = new Mock<IDbContext>();
            _controller = new EmployeeController(_mockDbContext.Object);
        }


        [Test]
        public void GivenLookingUpAResourceThatExistsThenReturnThatResourceProperlyMappedWithStatus200()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "bcd",
                FullName = "Fullname",
                Title = " title",
                Location = "location",
                Email = "email",
                PhoneNumber = "PhoneNumber",
                MobilePhoneNumber = "MobilePhoneNumber"
            };

            // .. mock queryable includes a record we _don't_ want, with a value that would be sorted first
            var list = new[] {user, new ApplicationUser {Id = "abc"}};

            var queryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(queryable.Object);

            // Act
            var result = _controller.Get(user.Id);

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeViewModel>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.name.Should().Be(user.FullName);
            okResult.Content.email.Should().Be(user.Email);
            okResult.Content.id.Should().Be(user.Id);
            okResult.Content.location.Should().Be(user.Location);
            okResult.Content.title.Should().Be(user.Title);
            okResult.Content.mobile.Should().Be(user.MobilePhoneNumber);
            okResult.Content.office.Should().Be(user.PhoneNumber);
        }

        [Test]
        public void GivenLookingUpAResourceThatDoesNotExistThenReturnStatus404()

        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "bcd",
                FullName = "Fullname",
                Title = " title",
                Location = "location",
                Email = "email",
                PhoneNumber = "PhoneNumber",
                MobilePhoneNumber = "MobilePhoneNumber"
            };

            // .. mock queryable includes a record we _don't_ want, with a value that would be sorted first
            var list = new[] { user, new ApplicationUser { Id = "abc" } };

            var queryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(queryable.Object);

            // Act
            var result = _controller.Get("1");

            // Assert
            result.Should()
                .BeOfType<NotFoundResult>();

        }
    }
}
