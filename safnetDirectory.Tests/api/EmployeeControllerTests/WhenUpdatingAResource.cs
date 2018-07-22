using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
    public class WhenUpdatingAResource
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
        public void GivenAResourceThatExistsThenUpdateTheDataLayerAndReturnStatus204()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "bcd",
            };

            // .. mock queryable includes a record we _don't_ want, with a value that would be sorted first
            const string otherName = "otherName";
            var list = new[] { user, new ApplicationUser { Id = "abc", FullName = otherName } };

            var queryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(queryable.Object);

            // .. allow data layer saving
            _mockDbContext.Setup(x => x.SaveChanges())
                .Returns(1);


            // .. finally the input object
            var viewModel = new EmployeeViewModel
            {
                id = user.Id,
                name = "name",
                office = "office",
                mobile = "mobile",
                location = "location",
                email = "email",
                title = "title"
            };

            // Act
            var result = _controller.Post(viewModel);

            // Assert
            result.Should()
                .BeOfType<StatusCodeResult>()
                .Subject
            .StatusCode
                .Should()
                .Be(HttpStatusCode.Accepted);

            // .. ensure that the user was actually updated
            user.FullName.Should().Be(viewModel.name);
            user.Location.Should().Be(viewModel.location);
            user.MobilePhoneNumber.Should().Be(viewModel.mobile);
            user.Title.Should().Be(viewModel.title);
            user.Email.Should().Be(viewModel.email);
            user.PhoneNumber.Should().Be(viewModel.office);

            // .. and the other user should be unchanged
            list[1].FullName.Should().Be(otherName);

            // .. ensure that the changes were persisted
            _mockDbContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public void GivenAResourceThatDoesNotExistThenReturnStatus404()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "bcd",
            };

            // .. mock queryable includes a record we _don't_ want, with a value that would be sorted first
            const string otherName = "otherName";
            var list = new[] { user, new ApplicationUser { Id = "abc", FullName = otherName } };

            var queryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(queryable.Object);


            // .. finally the input object
            var viewModel = new EmployeeViewModel
            {
                id = "WRONG ID",
                name = "name",
                office = "office",
                mobile = "mobile",
                location = "location",
                email = "email",
                title = "title"
            };

            // Act
            var result = _controller.Post(viewModel);

            // Assert
            result.Should()
                .BeOfType<NotFoundResult>();
        }

        [Test]
        public void GivenANullRequestBodyThenReturnStatusCode400()
        {
            // Arrange
            

            // Act
            var result = _controller.Post(null);

            // Assert
            result.Should()
                .BeOfType<BadRequestResult>();

        }

        [Test]
        public void GivenAnInvalidRequestBodyThenReturnStatusCode400()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "bcd",
            };

            // .. mock queryable includes a record we _don't_ want, with a value that would be sorted first
            const string otherName = "otherName";
            var list = new[] { user, new ApplicationUser { Id = "abc", FullName = otherName } };

            var queryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(queryable.Object);


            // .. finally the input object
            var viewModel = new EmployeeViewModel
            {
                id = user.Id
            };

            _controller.ModelState.AddModelError("anything", "something bad");

            // Act
            var result = _controller.Post(viewModel);

            // Assert
            result.Should()
                .BeOfType<InvalidModelStateResult>();
        }
    }
}
