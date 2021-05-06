using EmployeesApp.Application.Abstract;
using EmployeesApp.Application.Interfaces;
using EmployeesApp.Controllers;
using EmployeesApp.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EmployeesApp.UnitTests
{
    public class HomeControllerTests
    {
        private readonly HomeController _sut;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
        private readonly Mock<IRepository> _repositoryMock = new Mock<IRepository>();
        private readonly Mock<ILogger<HomeController>> _loggerMock = new Mock<ILogger<HomeController>>();
        private readonly Mock<ILoadCSVService> _loadCSVServiceMock = new Mock<ILoadCSVService>();

        public HomeControllerTests()
        {
            _sut = new HomeController(_loggerMock.Object, _repositoryMock.Object, _loadCSVServiceMock.Object);
        }

        [Fact]
        public void Index_ShouldReturnAListOfEmployeesViewResult()
        {
            // Arrange
            _repositoryMock.Setup(p => p.GetAll())
                .Returns(GetTestEmployees());

            // Act
            var result = _sut.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Employee>>(viewResult.Model);
            Assert.Equal(GetTestEmployees().Count, model.Count());
        }

        [Fact]
        public async Task Edit_ShouldReturnEmployeeViewResult()
        {
            // Arrange
            var employeeId = 1;
            var forenames = "John";
            var surname = "William";
            var employee = new Employee
            {
                Id = employeeId,
                ForeNames = forenames,
                Surname = surname
            };
            _repositoryMock.Setup(p => p.GetAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            var result = await _sut.Edit(employeeId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Employee>(viewResult.Model);
            Assert.Equal(employeeId, model.Id);
            Assert.Equal(forenames, model.ForeNames);
            Assert.Equal(surname, model.Surname);
        }

        [Fact]
        public async Task Edit_ShouldReturnBadRequestResultWhenIdIsNull()
        {
            // Arrange
            int? employeeId = null;

            // Act
            var result = await _sut.Edit(employeeId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Edit_ShouldReturnNotFoundResultWhenEmployeeNotFound()
        {
            // Arrange
            int? employeeId = 1;
            _repositoryMock.Setup(p => p.GetAsync(employeeId.Value))
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.Edit(employeeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task LoadCSV_ShouldReturnAddedRowsJsonResult()
        {
            // Arrange
            var dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\netcoreapp3.1", string.Empty));
            var testFilePath = Path.Combine(dirName, @"Assets\dataset.csv");
            using var streamReader = new StreamReader(testFilePath);
            using var stream = File.OpenRead(testFilePath);
            _loadCSVServiceMock.Setup(p => p.LoadFromCSVAsync(streamReader))
                .ReturnsAsync(2);

            // Act
            var result = await _sut.LoadCSV(new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(testFilePath)));

            // Assert
            Assert.IsType<JsonResult>(result);
        }

        private List<Employee> GetTestEmployees()
        {
            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    ForeNames = "John",
                    Surname = "William",
                },
                new Employee
                {
                    Id = 1,
                    ForeNames = "Jerry",
                    Surname = "Jackson",
                }
            };

            return employees;
        }
    }
}
