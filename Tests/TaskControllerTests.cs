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
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Tests
{
    public class TasksControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private ApplicationDbContext _context;

        public TasksControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            _context = new ApplicationDbContext(options);
        }

        public void Dispose()
        {

            _context.Dispose();
        }



        [Fact]
        public async Task Test1_IndexView()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Tasks/Index");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);


        }

        [Fact]
        public async Task Test2_CalendarView()
        {

            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Tasks/Calendar");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task Test3_Details_ValidId()
        {
            var task = new StudyFlow.Models.Domain.Task
            {
                Id = Guid.NewGuid(),
                Title = "Sample Task",
                Description = "This is a sample task",
                Priority = Priority.High,
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),

            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            var controller = new TasksController(_context);
            var result = await controller.Details(task.Id);

            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.Equal(task, viewResult.Model);
        }

        [Fact]
        public async Task Test4_Details_InvalidId()
        {

            var task = new StudyFlow.Models.Domain.Task
            {
                Id = Guid.NewGuid(),
                Title = "Sample Task",
                Description = "This is a sample task",
                Priority = Priority.High,
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7)
            };


            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();


            var controller = new TasksController(_context);
            var invalidId = Guid.NewGuid();
            var result = await controller.Details(invalidId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test5_Create_Task()
        {
            var user = new StudyFlowUser
            {
                Id = Guid.NewGuid().ToString()
            };


            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var task = new StudyFlow.Models.Domain.Task
            {
                Id = Guid.NewGuid(),
                Title = "Sample Task",
                Description = "This is a sample task",
                Priority = Priority.High,
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            var controller = new TasksController(_context);
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id)
             };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = await controller.Create(task);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(Index), redirectResult.ActionName);

        }

        [Fact]
        public async Task Test6_Edit_Task()
        {
            var task = new StudyFlow.Models.Domain.Task
            {
                Id = Guid.NewGuid(),
                Title = "Sample Task",
                Description = "This is a sample task",
                Priority = Priority.High,
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            var controller = new TasksController(_context);
            var result = await controller.Edit(task.Id);

            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.Equal(task, viewResult.Model);
        }

        [Fact]
        public async Task Test7_Delete_Task()
        {
            var task = new StudyFlow.Models.Domain.Task
            {
                Id = Guid.NewGuid(),
                Title = "Sample Task",
                Description = "This is a sample task",
                Priority = Priority.High,
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7)
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            var controller = new TasksController(_context);

            var result = await controller.Delete(task.Id);

            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.Equal(task, viewResult.Model);
        }

    }
}

