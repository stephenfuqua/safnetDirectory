using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using safnetDirectory.FullMvc.api;
using safnetDirectory.FullMvc.Data;

namespace safnetDirectory.FullMvc.Tests.api.EmployeeControllerTests
{
    [TestFixture]
    public class WhenConstructingTheController
    {
        [Test]
        public void GivenValidArgumentsThenNoErrors()
        {
            Action act = () =>
            {
                var ignore = new EmployeeController(Mock.Of<IDbContext>());
            };

            act.Should().NotThrow();
        }

        [Test]
        public void GivenNullArgumentThenThrowArgumentNullException()
        {
            Action act = () =>
            {
                var ignore = new EmployeeController(null);
            };

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
