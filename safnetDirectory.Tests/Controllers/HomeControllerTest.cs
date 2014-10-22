using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using safnetDirectory;
using safnetDirectory.FullMvc.Controllers;

namespace safnetDirectory.FullMvc.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            var controller = GivenTheSystemUnderTest();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            var controller = GivenTheSystemUnderTest();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
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

        [TestMethod]
        public void EmployeePagingDefaultParameters()
        {
            var controller = GivenTheSystemUnderTest();

            var result = controller.EmployeePaging();

            Assert.IsNotNull(result, "result is null");
            Assert.IsNotNull(result.Data, "data is null");

            var data = result.Data as List<EmployeeViewModel>;

            Assert.AreEqual(10, data.Count(), "employee count");
            Assert.AreEqual("\"Herb J. Wesson, Jr.\"", data.First().name, "first name in the list");
        }

        [TestMethod]
        public void EmployeePagingWithAlternatePageSize()
        {
            var controller = GivenTheSystemUnderTest();

            var result = controller.EmployeePaging(pageSize: 20);

            Assert.IsNotNull(result, "result is null");
            Assert.IsNotNull(result.Data, "data is null");

            var data = result.Data as List<EmployeeViewModel>;

            Assert.AreEqual(20, data.Count(), "employee count");
            Assert.AreEqual("\"Herb J. Wesson, Jr.\"", data.First().name, "first name in the list");
        }

        [TestMethod]
        public void EmployeePagingWithAlternatePageNumber()
        {
            var controller = GivenTheSystemUnderTest();

            var result = controller.EmployeePaging(page: 2);

            Assert.IsNotNull(result, "result is null");
            Assert.IsNotNull(result.Data, "data is null");

            var data = result.Data as List<EmployeeViewModel>;

            Assert.AreEqual(10, data.Count(), "employee count");
            Assert.AreEqual("Abdssamad Said ", data.First().name, "first name in the list");
        }

        [TestMethod]
        public void EmployeePagingWithFilterByFullName()
        {
            var controller = GivenTheSystemUnderTest();

            //var filter = "{\"name\":\"my name\",\"title\":\"my title\",\"location\":\"my location\",\"email\":\"my email\"}";
            var filter = "{\"name\":\"Abdssamad Said\"}";

            var result = controller.EmployeePaging(searchText: filter);

            Assert.IsNotNull(result, "result is null");
            Assert.IsNotNull(result.Data, "data is null");

            var data = result.Data as List<EmployeeViewModel>;

            Assert.AreEqual(1, data.Count(), "employee count");
            Assert.AreEqual("Abdssamad Said", data.First().name, "first name in the list");
        }

        [TestMethod]
        public void EmployeePagingWithFilterByFullName_Partialentry()
        {
            var controller = GivenTheSystemUnderTest();

            //var filter = "{\"name\":\"my name\",\"title\":\"my title\",\"location\":\"my location\",\"email\":\"my email\"}";
            var filter = "{\"name\":\"Abee\"}";

            var result = controller.EmployeePaging(searchText: filter);

            Assert.IsNotNull(result, "result is null");
            Assert.IsNotNull(result.Data, "data is null");

            var data = result.Data as List<EmployeeViewModel>;

            Assert.AreEqual(4, data.Count(), "employee count");
            Assert.AreEqual("Abee Elizabeth Lutz", data.First().name, "first name in the list");
        }

        [TestMethod]
        public void EmployeePagingWithFilterByTitle()
        {
            var controller = GivenTheSystemUnderTest();

            //var filter = "{\"name\":\"my name\",\"title\":\"my title\",\"location\":\"my location\",\"email\":\"my email\"}";
            var filter = "{\"title\":\"Programmer\"}";

            var result = controller.EmployeePaging(searchText: filter);

            Assert.IsNotNull(result, "result is null");
            Assert.IsNotNull(result.Data, "data is null");

            var data = result.Data as List<EmployeeViewModel>;

            Assert.AreEqual(10, data.Count(), "employee count");
            Assert.AreEqual("Abbott, Jarvis", data.First().name, "first name in the list");
        }

        [TestMethod]
        public void EmployeePagingWithFilterByLocation()
        {
            var controller = GivenTheSystemUnderTest();

            //var filter = "{\"name\":\"my name\",\"title\":\"my title\",\"location\":\"my location\",\"email\":\"my email\"}";
            var filter = "{\"location\":\"TX\"}";

            var result = controller.EmployeePaging(searchText: filter);

            Assert.IsNotNull(result, "result is null");
            Assert.IsNotNull(result.Data, "data is null");

            var data = result.Data as List<EmployeeViewModel>;

            Assert.AreEqual(10, data.Count(), "employee count");
            Assert.AreEqual("Lee John Warren", data.First().name, "first name in the list");
        }

        [TestMethod]
        public void EmployeePagingWithFilterByEmail()
        {
            var controller = GivenTheSystemUnderTest();

            //var filter = "{\"name\":\"my name\",\"title\":\"my title\",\"location\":\"my location\",\"email\":\"my email\"}";
            var filter = "{\"email\":\"lee.randy.p\"}";

            var result = controller.EmployeePaging(searchText: filter);

            Assert.IsNotNull(result, "result is null");
            Assert.IsNotNull(result.Data, "data is null");

            var data = result.Data as List<EmployeeViewModel>;

            Assert.AreEqual(1, data.Count(), "employee count");
            Assert.AreEqual("Lee Randy P", data.First().name, "first name in the list");
        }

        [TestMethod]
        public void EmployeePagingFilterByAllFields()
        {
            var controller = GivenTheSystemUnderTest();

            var filter = "{\"name\":\"Sh\",\"title\":\"Eng\",\"location\":\"orpus\",\"email\":\"denise\"}";

            var result = controller.EmployeePaging(searchText: filter);

            Assert.IsNotNull(result, "result is null");
            Assert.IsNotNull(result.Data, "data is null");

            var data = result.Data as List<EmployeeViewModel>;

            Assert.AreEqual(10, data.Count(), "employee count");
            Assert.AreEqual("Lee Sharron Denise", data.First().name, "first name in the list");
        }

        [TestMethod]
        public void TotalNumberOfRecords()
        {
            var controller = GivenTheSystemUnderTest();

            var result = controller.TotalNumberOfEmployees();

            Assert.IsNotNull(result, "result is null");
            Assert.IsNotNull(result.Data, "data is null");

            var data = result.Data as int?;

            Assert.AreEqual(30017, data, "record count");
        }
    }
}
