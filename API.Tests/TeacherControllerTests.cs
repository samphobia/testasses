using API.Controllers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests
{
    public class TeachersControllerTests
    {
        [Fact]
        public async Task GetUsers_ReturnsListOfTeachers()
        {
            // Arrange
            var teachers = new List<AppTeacher>
               {
                    new AppTeacher("123456", "Mr", "John", "Doe", new DateTime(1990, 1, 1), "T123", 50000),
                    new AppTeacher("789012", "Mrs", "Alice", "Smith", new DateTime(1985, 3, 15), "T456", 55000)
                }.AsQueryable();

            var mockDbContext = new Mock<DataContext>();
            mockDbContext.Setup(db => db.Teachers).ReturnsDbSet(teachers);

            var controller = new TeachersController(mockDbContext.Object);

            // Act
            var result = await controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTeachers = Assert.IsAssignableFrom<IEnumerable<AppTeacher>>(okResult.Value);
            Assert.Equal(2, returnedTeachers.Count());
        }

            [Fact]
            public async Task GetUser_WithValidId_ReturnsTeacher()
            {
                // Arrange
                var teachers = new List<AppTeacher>
                {
                    new AppTeacher { Id = 1, Name = "John" },
                    new AppTeacher { Id = 2, Name = "Alice" }
                }.AsQueryable();

                var mockDbContext = new Mock<DataContext>();
                mockDbContext.Setup(db => db.Teachers).ReturnsDbSet(teachers);

                var controller = new TeachersController(mockDbContext.Object);

                // Act
                var result = await controller.GetUser(1);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var teacher = Assert.IsAssignableFrom<AppTeacher>(okResult.Value);
                Assert.Equal("John", teacher.Name);
            }

            [Fact]
            public async Task GetUser_WithInvalidId_ReturnsNotFound()
            {
                // Arrange
                var teachers = new List<AppTeacher>
                {
                    new AppTeacher { Id = 1, Name = "John" },
                    new AppTeacher { Id = 2, Name = "Alice" }
                }.AsQueryable();

                var mockDbContext = new Mock<DataContext>();
                mockDbContext.Setup(db => db.Teachers).ReturnsDbSet(teachers);

                var controller = new TeachersController(mockDbContext.Object);

                // Act
                var result = await controller.GetUser(3);

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }

            [Fact]
            public async Task PostTeacher_WithValidModel_CreatesTeacher()
            {
                // Arrange
                var newTeacher = new AppTeacher
                {
                    NationalID = "12345",
                    Title = "Mr",
                    Name = "John",
                    Surname = "Doe",
                    DateOfBirth = new System.DateTime(1990, 1, 1),
                    TeacherNumber = "T123",
                    Salary = 50000
                };

                var mockDbContext = new Mock<DataContext>();
                mockDbContext.Setup(db => db.Teachers.AddAsync(newTeacher, default(CancellationToken)))
                             .Returns(Task.FromResult(newTeacher));

                var controller = new TeachersController(mockDbContext.Object);
                controller.ModelState.AddModelError("error", "some error");

                // Act
                var result = await controller.PostTeacher(newTeacher);

                // Assert
                var createdResult = Assert.IsType<CreatedAtActionResult>(result);
                var teacher = Assert.IsAssignableFrom<AppTeacher>(createdResult.Value);
                Assert.Equal("John", teacher.Name);
            }
    }

    // Extension method to mock DbSet using IQueryable
    public static class DbSetExtensions
    {
        public static DbSet<T> ReturnsDbSet<T>(this IQueryable<T> queryable) where T : class
        {
            var mock = new Mock<DbSet<T>>();
            mock.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mock.Object;
        }
    }
}
