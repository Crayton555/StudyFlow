using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using StudyFlow.Controllers;
using StudyFlow.Data;
using StudyFlow.Models;
using System;
using System.Threading.Tasks;

namespace StudyFlow.Tests.Controllers
{
    [TestFixture]
    public class CalendarControllerTests
    {
        private ApplicationDbContext _dbContext;
        private CalendarController _controller;

        [SetUp]
        public void Setup()
        {
            // Initialize an in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureCreated();

            // Create the controller with the test database context
            _controller = new CalendarController(_dbContext);
        }

        [TearDown]
        public void Teardown()
        {
            // Dispose the test database context and delete the in-memory database
            _dbContext.Dispose();
            _dbContext = null;
        }

        [Test]
        public async Task Index_WithNonNullCalendar_ReturnsViewWithCalendarData()
        {
            // Arrange
            var calendar = new Calendar { Id = Guid.NewGuid(), Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as Calendar[];
            Assert.NotNull(model);
            Assert.AreEqual(1, model.Length);
            Assert.AreEqual(calendar.Id, model[0].Id);
        }

        [Test]
        public async Task Index_WithNullCalendar_ReturnsProblemResult()
        {
            // Arrange
            _dbContext.Calendar = null;

            // Act
            var result = await _controller.Index() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            Assert.AreEqual("Entity set 'ApplicationDbContext.Class'  is null.", result.Value);
        }

        [Test]
        public async Task Details_WithValidId_ReturnsViewWithCalendarData()
        {
            // Arrange
            var calendarId = Guid.NewGuid();
            var calendar = new Calendar { Id = calendarId, Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.Details(calendarId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as Calendar;
            Assert.NotNull(model);
            Assert.AreEqual(calendarId, model.Id);
        }

        [Test]
        public async Task Details_WithNullId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var calendarId = Guid.NewGuid();
            var calendar = new Calendar { Id = calendarId, Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            // Act
            var invalidId = Guid.NewGuid();
            var result = await _controller.Details(invalidId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Create_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var calendar = new Calendar { Id = Guid.NewGuid(), Title = "Test Event", Date = DateTime.Now, Description = "Test description" };

            // Act
            var result = await _controller.Create(calendar) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var createdCalendar = await _dbContext.Calendar.FindAsync(calendar.Id);
            Assert.NotNull(createdCalendar);
            Assert.AreEqual(calendar.Id, createdCalendar.Id);
        }

        [Test]
        public async Task Create_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var calendar = new Calendar { Id = Guid.NewGuid(), Title = null, Date = DateTime.Now, Description = "Test description" };
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act
            var result = await _controller.Create(calendar) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as Calendar;
            Assert.NotNull(model);
            Assert.AreEqual(calendar.Id, model.Id);
        }

        [Test]
        public async Task Edit_WithValidId_ReturnsViewWithCalendarData()
        {
            // Arrange
            var calendarId = Guid.NewGuid();
            var calendar = new Calendar { Id = calendarId, Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.Edit(calendarId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as Calendar;
            Assert.NotNull(model);
            Assert.AreEqual(calendarId, model.Id);
        }

        [Test]
        public async Task Edit_WithNullId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit(null);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var calendarId = Guid.NewGuid();
            var calendar = new Calendar { Id = calendarId, Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            // Act
            var invalidId = Guid.NewGuid();
            var result = await _controller.Edit(invalidId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var calendarId = Guid.NewGuid();
            var calendar = new Calendar { Id = calendarId, Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.Edit(calendarId, calendar) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var updatedCalendar = await _dbContext.Calendar.FindAsync(calendarId);
            Assert.NotNull(updatedCalendar);
            Assert.AreEqual(calendar.Title, updatedCalendar.Title);
        }

        [Test]
        public async Task Edit_WithInvalidModelId_ReturnsNotFound()
        {
            // Arrange
            var calendarId = Guid.NewGuid();
            var calendar = new Calendar { Id = calendarId, Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            var invalidId = Guid.NewGuid();

            // Act
            var result = await _controller.Edit(invalidId, calendar);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var calendarId = Guid.NewGuid();
            var calendar = new Calendar { Id = calendarId, Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            var updatedCalendar = new Calendar { Id = calendarId, Title = null, Date = DateTime.Now, Description = "Test description" };
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act
            var result = await _controller.Edit(calendarId, updatedCalendar) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as Calendar;
            Assert.NotNull(model);
            Assert.AreEqual(calendarId, model.Id);
        }

        [Test]
        public async Task Delete_WithValidId_ReturnsViewWithCalendarData()
        {
            // Arrange
            var calendarId = Guid.NewGuid();
            var calendar = new Calendar { Id = calendarId, Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.Delete(calendarId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as Calendar;
            Assert.NotNull(model);
            Assert.AreEqual(calendarId, model.Id);
        }

        [Test]
        public async Task Delete_WithNullId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var calendarId = Guid.NewGuid();
            var calendar = new Calendar { Id = calendarId, Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            // Act
            var invalidId = Guid.NewGuid();
            var result = await _controller.Delete(invalidId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteConfirmed_WithValidId_RemovesCalendarAndRedirectsToIndex()
        {
            // Arrange
            var calendarId = Guid.NewGuid();
            var calendar = new Calendar { Id = calendarId, Title = "Test Event", Date = DateTime.Now, Description = "Test description" };
            _dbContext.Calendar.Add(calendar);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.DeleteConfirmed(calendarId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var deletedCalendar = await _dbContext.Calendar.FindAsync(calendarId);
            Assert.Null(deletedCalendar);
        }

        [Test]
        public async Task DeleteConfirmed_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var invalidId = Guid.NewGuid();
            var result = await _controller.DeleteConfirmed(invalidId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
