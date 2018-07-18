using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;
using safnetDirectory;
using safnetDirectory.FullMvc.Controllers;
using safnetDirectory.FullMvc.Models;

namespace safnetDirectory.FullMvc.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
         [Test]
        public void Index()
        {
            var controller = GivenTheSystemUnderTest();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

         [Test]
        public void About()
        {
            var controller = GivenTheSystemUnderTest();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

         [Test]
        public void Contact()
        {
            // Arrange
            var controller = GivenTheSystemUnderTest();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        private static HomeController GivenTheSystemUnderTest()
        {
            HomeController controller = new HomeController();
            return controller;
        }

      

    }
}
