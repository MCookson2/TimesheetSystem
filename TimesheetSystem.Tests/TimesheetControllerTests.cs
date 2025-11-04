using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TimesheetSystem.API.Controllers;
using TimesheetSystem.Common.Classes;
using TimesheetSystem.Common.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimesheetSystem.Tests
{
    public class TimesheetControllerTests
    {
        private readonly Mock<ILogger<TimesheetController>> _loggerMock = new();
        private readonly Mock<ITimesheetService> _serviceMock = new();

        [Fact]
        public void GetHoursPerProject_ReturnsOk_WithEntries()
        {
            DateTime today = DateTime.Today;

            Dictionary<int, double> expectedEntries = new Dictionary<int, double>
            {
                { 1, 20 },
                { 2, 2.5 },
                { 3, 1 },
                { 4, 11 },
                { 5, 0.5 }
            };

            _serviceMock
                .Setup(service => service.GetHoursPerProject(1, today))
                .Returns(expectedEntries);

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);

            var result = controller.GetHoursPerProject(1, today);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IDictionary<int, double>>(okResult.Value);
            Assert.Equal(expectedEntries.Count, data.Count);
        }

        [Fact]
        public void GetHoursPerProject_WhenServiceThrows_ReturnsInternalServerError()
        {
            DateTime today = DateTime.Today;

            Dictionary<int, double> expectedEntries = new Dictionary<int, double>
            {
                { 1, 20 },
                { 2, 2.5 },
                { 3, 1 },
                { 4, 11 },
                { 5, 0.5 }
            };

            _serviceMock
                .Setup(service => service.GetHoursPerProject(1, today))
                .Throws(new Exception("Database error"));

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);

            var result = controller.GetHoursPerProject(1, today);

            StatusCodeResult statusCodeResult = Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public void GetAllEntriesForUserPerWeek_ReturnsOk_WithEntries()
        {
            DateTime today = DateTime.Today;

            Dictionary<int, TimesheetEntry> expectedEntries = new Dictionary<int, TimesheetEntry>
            {
                { 1, new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 8, Date = today } },
                { 2, new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 5, Date = today } },
                { 3, new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 7, Date = today } },
                { 4, new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 1, Date = today } },
                { 5, new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 1, Date = today } },
            };

            _serviceMock
                .Setup(service => service.GetAllEntriesForUserPerWeek(1, today))
                .Returns(expectedEntries);

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);

            var result = controller.GetAllEntriesForUserPerWeek(1, today);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IDictionary<int, TimesheetEntry>>(okResult.Value);
            Assert.Equal(expectedEntries.Count, data.Count);
        }

        [Fact]
        public void GetAllEntriesForUserPerWeek_WhenServiceThrows_ReturnsInternalServerError()
        {
            DateTime today = DateTime.Today;

            Dictionary<int, TimesheetEntry> expectedEntries = new Dictionary<int, TimesheetEntry>
            {
                { 1, new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 8 } }
            };

            _serviceMock
                .Setup(service => service.GetAllEntriesForUserPerWeek(1, today))
                .Throws(new Exception("Database error"));

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);

            var result = controller.GetAllEntriesForUserPerWeek(1, today);

            StatusCodeResult statusCodeResult = Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public void GetAllEntries_ReturnsOk_WithEntries()
        {
            Dictionary<int, TimesheetEntry> expectedEntries = new Dictionary<int, TimesheetEntry>
            {
                { 1, new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 8 } }
            };

            _serviceMock
                .Setup(service => service.GetAllEntries(0, 10))
                .Returns(expectedEntries);

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);

            var result = controller.GetAllEntries(0, 10);

            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IDictionary<int, TimesheetEntry>>(okResult.Value);
            Assert.Single(data);
        }

        [Fact]
        public void GetAllEntries_WhenServiceThrows_ReturnsInternalServerError()
        {
            Dictionary<int, TimesheetEntry> expectedEntries = new Dictionary<int, TimesheetEntry>
            {
                { 1, new TimesheetEntry { UserID = 1, ProjectID = 10, HoursWorked = 8 } }
            };

            _serviceMock
                .Setup(service => service.GetAllEntries(0, 10))
                .Throws(new Exception("Database error"));

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);

            var result = controller.GetAllEntries(0, 10);

            StatusCodeResult statusCodeResult = Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public void Add_CallsServiceAndReturnsOk()
        {
            TimesheetEntry entry = new TimesheetEntry { UserID = 1, ProjectID = 20, HoursWorked = 5 };
            _serviceMock.Setup(s => s.Add(entry));

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);

            ActionResult result = controller.Add(entry);

            OkResult ok = Assert.IsType<OkResult>(result);
            _serviceMock.Verify(s => s.Add(It.IsAny<TimesheetEntry>()), Times.Once);
        }

        [Fact]
        public void Add_WhenServiceThrows_ReturnsInternalServerError()
        {
            int id = 1;
            TimesheetEntry entry = new TimesheetEntry { UserID = 1, ProjectID = 20, HoursWorked = 5 };

            _serviceMock
                .Setup(s => s.Add(entry))
                .Throws(new Exception("Database error"));

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);

            ActionResult result = controller.Add(entry);

            StatusCodeResult statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public void Update_CallsServiceAndReturnsOk()
        {
            int id = 1;
            TimesheetEntry entry = new TimesheetEntry { UserID = 1, ProjectID = 20, HoursWorked = 5 };
            _serviceMock.Setup(s => s.Update(id, entry));

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);
            ActionResult result = controller.Update(id, entry);

            OkResult ok = Assert.IsType<OkResult>(result);
            _serviceMock.Verify(s => s.Update(id, entry), Times.Once);
        }

        [Fact]
        public void Update_WhenServiceThrows_ReturnsInternalServerError()
        {
            int id = 1;
            TimesheetEntry entry = new TimesheetEntry { UserID = 1, ProjectID = 20, HoursWorked = 5 };

            _serviceMock
                .Setup(s => s.Update(id, entry))
                .Throws(new Exception("Database error"));

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);

            ActionResult result = controller.Update(id, entry);

            StatusCodeResult statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public void Delete_CallsServiceAndReturnsOk()
        {
            int id = 1;
            _serviceMock.Setup(s => s.Delete(id));

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);
            ActionResult result = controller.Delete(id);

            OkResult ok = Assert.IsType<OkResult>(result);
            _serviceMock.Verify(s => s.Delete(id), Times.Once);
        }

        [Fact]
        public void Delete_WhenServiceThrows_ReturnsInternalServerError()
        {
            int id = 1;

            _serviceMock
                .Setup(s => s.Delete(id))
                .Throws(new Exception("Database error"));

            TimesheetController controller = new TimesheetController(_loggerMock.Object, _serviceMock.Object);

            ActionResult result = controller.Delete(id);

            StatusCodeResult statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
