using Microsoft.AspNetCore.Mvc.Testing;
using System.Reflection;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
using StudyFlow;
using Microsoft.Extensions.DependencyInjection;
using StudyFlow.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using StudyFlow.Controllers;
using StudyFlow.Models.Domain.Enumeration;
using Xunit;
using Xunit.Sdk;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using StudyFlow.Models.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using StudyFlow.Models;
using System.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Tests
{
    public class HomeControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HomeControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }



        [Fact]
        public void Test1_Index()
        {
            var controller = new HomeController(Mock.Of<ILogger<HomeController>>());
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Test2_Privacy()
        {
            var controller = new HomeController(Mock.Of<ILogger<HomeController>>());
            var result = controller.Privacy();

            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.Null(viewResult.ViewName);
        }


    }
}