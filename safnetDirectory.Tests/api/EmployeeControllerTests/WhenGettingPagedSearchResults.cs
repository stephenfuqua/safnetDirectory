using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http.Results;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using safnetDirectory.FullMvc.api;
using safnetDirectory.FullMvc.Controllers;
using safnetDirectory.FullMvc.Data;
using safnetDirectory.FullMvc.Models;

namespace safnetDirectory.FullMvc.Tests.api.EmployeeControllerTests
{
    [TestFixture]
    public class WhenGettingPagedSearchResults
    {
        private EmployeeController _controller;
        private Mock<IDbContext> _mockDbContext;

        [SetUp]
        public void SetUp()
        {
            _mockDbContext = new Mock<IDbContext>();
            _controller = new EmployeeController(_mockDbContext.Object);

            // Setup the current user as an HR user so that Id values will be mapped for editing
            var mockPrincipal = new Mock<IPrincipal>();

            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.Claims)
                .Returns(new[] { new Claim(ClaimTypes.Role, AdminController.HR_ROLE) });

            mockPrincipal.Setup(x => x.Identity)
                .Returns(mockIdentity.Object);

            _controller.User = mockPrincipal.Object;
        }

        [Test]
        public void GivenThereAreRecordsAndTheRequestorIsNotAnHrUserThenReturnThoseRecordsWithIdRemoved()
        {
            // Arrange
            // ... override the default _claims collection so that HR is not returned
            var mockPrincipal = new Mock<IPrincipal>();

            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(x => x.Claims)
                .Returns(new[] { new Claim(ClaimTypes.Role, AdminController.USER_ROLE) });

            mockPrincipal.Setup(x => x.Identity)
                .Returns(mockIdentity.Object);

            _controller.User = mockPrincipal.Object;


            // ... setup the data retrieval
            var list = new List<ApplicationUser>
            {
                new ApplicationUser {Id = "1", FullName = "a"},
                new ApplicationUser {Id = "2", FullName = "b"}
            };
            var mockQueryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(mockQueryable.Object);

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeList>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.Employees.Should().NotBeNull().And.HaveCount(2);

            okResult.Content.Employees.First().id.Should().BeNullOrEmpty();
            okResult.Content.Employees.Skip(1).First().id.Should().BeNullOrEmpty();

            // MockQueryable
            okResult.Content.TotalRecords.Should().Be(2);
        }

        [Test]
        public void GivenNoEmployeesWhenQueryingWithDefaultParametersThenReturnEmptyListAndCountZero()
        {
            // Arrange
            var mockQueryable = new List<ApplicationUser>().AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(mockQueryable.Object);

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeList>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.Employees.Should().NotBeNull().And.BeEmpty();
            okResult.Content.TotalRecords.Should().Be(0);
        }

        [Test]
        public void GivenThreeEmployeesWhenQueryingWithPageSizeTwoThenReturnTwoItemsSortedByNameAndCountThree()
        {
            // Arrange
            var list = new List<ApplicationUser>
            {
                new ApplicationUser {Id = "1", FullName = "a"},
                new ApplicationUser {Id = "2", FullName = "b"},
                new ApplicationUser {Id = "3", FullName = "c"}
            };
            var mockQueryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(mockQueryable.Object);

            // Act
            var result = _controller.Get(pageSize: 2);

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeList>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.Employees.Should().NotBeNull().And.HaveCount(2);

            okResult.Content.Employees.First().id.Should().Be(list[0].Id);
            okResult.Content.Employees.Skip(1).First().id.Should().Be(list[1].Id);

            // MockQueryable
            okResult.Content.TotalRecords.Should().Be(3);
        }

        [Test]
        public void GivenThreeEmployeesWhenQueryingWithPageSizeTwoAndSecondPageThenReturnOneItemAndCountThree()
        {
            // Arrange
            var list = new List<ApplicationUser>
            {
                new ApplicationUser {Id = "1", FullName = "b"},
                new ApplicationUser {Id = "2", FullName = "d"},
                new ApplicationUser {Id = "3", FullName = "c"}
            };
            var mockQueryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(mockQueryable.Object);


            // Act
            var result = _controller.Get(pageSize: 2, page: 2);

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeList>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.Employees.Should().NotBeNull().And.HaveCount(1);

            okResult.Content.Employees.First().id.Should().Be(list[2].Id);

            okResult.Content.TotalRecords.Should().Be(3);
        }

        [Test]
        public void GivenThreeEmployeesAndTwoHavePartiallySameNameWhenQueryingByNameThenReturnTheTwoSortedByNameAndCountThree()
        {
            // Arrange
            var list = new List<ApplicationUser>
            {
                new ApplicationUser {Id = "1", FullName = "Person One"},
                new ApplicationUser {Id = "2", FullName = "Another One"},
                new ApplicationUser {Id = "3", FullName = "Person Three"}
            };
            var mockQueryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(mockQueryable.Object);


            // Act
            var result = _controller.Get(new EmployeeViewModel { name = "Person" });

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeList>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.Employees.Should().NotBeNull().And.HaveCount(2);

            okResult.Content.Employees.First().id.Should().Be(list[0].Id);
            okResult.Content.Employees.Skip(1).First().id.Should().Be(list[2].Id);

            okResult.Content.TotalRecords.Should().Be(3);
        }

        [Test]
        public void GivenThreeEmployeesWhenQueryingByTitleThenReturnOneEntryAndCountThree()
        {
            // Arrange
            var list = new List<ApplicationUser>
            {
                new ApplicationUser {Id = "1", Title = "Pers0n"},
                new ApplicationUser {Id = "2", Title = "Person"},
                new ApplicationUser {Id = "3", Title = "Per5on"}
            };
            var mockQueryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(mockQueryable.Object);


            // Act
            var result = _controller.Get(new EmployeeViewModel { title = "Person" });

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeList>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.Employees.Should().NotBeNull().And.HaveCount(1);

            okResult.Content.Employees.First().id.Should().Be(list[1].Id);

            okResult.Content.TotalRecords.Should().Be(3);
        }

        [Test]
        public void GivenThreeEmployeesWhenQueryingByEmailThenReturnOneEntryAndCountThree()
        {
            // Arrange
            var list = new List<ApplicationUser>
            {
                new ApplicationUser {Id = "1", Email = "Pers0n"},
                new ApplicationUser {Id = "2", Email = "Person"},
                new ApplicationUser {Id = "3", Email = "Per5on"}
            };
            var mockQueryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(mockQueryable.Object);


            // Act
            var result = _controller.Get(new EmployeeViewModel { email = "Person" });

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeList>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.Employees.Should().NotBeNull().And.HaveCount(1);

            okResult.Content.Employees.First().id.Should().Be(list[1].Id);

            okResult.Content.TotalRecords.Should().Be(3);
        }

        [Test]
        public void GivenThreeEmployeesWhenQueryingByLocationThenReturnOneEntryAndCountThree()
        {
            // Arrange
            var list = new List<ApplicationUser>
            {
                new ApplicationUser {Id = "1", Location = "Pers0n"},
                new ApplicationUser {Id = "2", Location = "Person"},
                new ApplicationUser {Id = "3", Location = "Per5on"}
            };
            var mockQueryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(mockQueryable.Object);


            // Act
            var result = _controller.Get(new EmployeeViewModel { location = "Person" });

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeList>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.Employees.Should().NotBeNull().And.HaveCount(1);

            okResult.Content.Employees.First().id.Should().Be(list[1].Id);

            okResult.Content.TotalRecords.Should().Be(3);
        }

        [Test]
        public void GivenOneEmployeeWhenQueryingForAllThenReturnedEmployeeHasAllPropertiesMappedCorrectly()
        {
            // Arrange
            var list = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "2",
                    Email = "a@b.com",
                    FullName = "asdfasdf",
                    Title = "89798798",
                    Location = "89779",
                    PhoneNumber = "67890",
                    MobilePhoneNumber = "456789"
                }
            };
            var mockQueryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(mockQueryable.Object);


            // Act
            var result = _controller.Get();

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeList>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.Employees.Should().NotBeNull().And.HaveCount(1);

            okResult.Content.Employees.First().id.Should().Be(list[0].Id);
            okResult.Content.Employees.First().email.Should().Be(list[0].Email);
            okResult.Content.Employees.First().name.Should().Be(list[0].FullName);
            okResult.Content.Employees.First().title.Should().Be(list[0].Title);
            okResult.Content.Employees.First().location.Should().Be(list[0].Location);
            okResult.Content.Employees.First().office.Should().Be(list[0].PhoneNumber);
            okResult.Content.Employees.First().mobile.Should().Be(list[0].MobilePhoneNumber);
        }

        [Test]
        public void GivenFiveEmployeesWhenFilteringByAllFourFieldsThenReturnOneEmployeeAndCountOne()
        {
            // Arrange
            var list = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "2", Email = "a@b.com", FullName = "asdfasdf", Title = "89798798", Location = "89779", },
                new ApplicationUser { Id = "3", Email = "a@b.com2", FullName = "asdfasdf3", Title = "897987987", Location = "897791", },
                new ApplicationUser { Id = "4", Email = "a@b.com3", FullName = "asdfasdf4", Title = "897987988", Location = "897792", },
                new ApplicationUser { Id = "5", Email = "a@b.com4", FullName = "asdfasdf5", Title = "897987989", Location = "897793", },
                new ApplicationUser { Id = "6", Email = "a@b.com5", FullName = "asdfasdf6", Title = "897987980", Location = "89779__", },
            };
            var mockQueryable = list.AsQueryable().BuildMock();

            _mockDbContext.Setup(x => x.Users)
                .Returns(mockQueryable.Object);


            // Act
            var result = _controller.Get(new EmployeeViewModel
            {
                name = "asdfasdf",
                title = "89798798",
                email = "a@b.com",
                location = "_"
            });

            // Assert
            var okResult = result.Should()
                .BeOfType<OkNegotiatedContentResult<EmployeeList>>()
                .Subject;

            okResult.Content.Should().NotBeNull();
            okResult.Content.Employees.Should().NotBeNull().And.HaveCount(1);

            okResult.Content.Employees.First().id.Should().Be(list[4].Id);
        }
    }
}
