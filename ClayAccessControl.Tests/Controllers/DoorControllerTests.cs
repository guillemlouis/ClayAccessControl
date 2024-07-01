using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ClayAccessControl.API.Controllers;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Exceptions;
using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClayAccessControl.Tests.Controllers
{
    public class DoorControllerTests
    {
        private readonly Mock<IDoorService> _mockDoorService;
        private readonly Mock<ILogger<DoorController>> _mockLogger;
        private readonly DoorController _controller;

        public DoorControllerTests()
        {
            _mockDoorService = new Mock<IDoorService>();
            _mockLogger = new Mock<ILogger<DoorController>>();
            _controller = new DoorController(_mockDoorService.Object, _mockLogger.Object);

            // Setup ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Setup HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.User = claimsPrincipal;
            httpContext.Items["UserId"] = 1; // Simulating UserIdFilter

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetDoors_ReturnsOkResult_WithListOfDoors()
        {
            // Arrange
            var doors = new List<DoorDto>
            {
                new DoorDto { DoorId = 1, DoorName = "Door 1" },
                new DoorDto { DoorId = 2, DoorName = "Door 2" }
            };
            _mockDoorService.Setup(service => service.GetAllDoorsAsync(1))
                .ReturnsAsync(doors);

            // Act
            var result = await _controller.GetDoors();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<DoorDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedDoors = Assert.IsType<List<DoorDto>>(okResult.Value);
            Assert.Equal(2, returnedDoors.Count);
        }

        [Fact]
        public async Task GetDoor_WithValidId_ReturnsOkResult_WithDoor()
        {
            // Arrange
            var doorDto = new DoorDto { DoorId = 1, DoorName = "Test Door" };
            _mockDoorService.Setup(service => service.GetDoorByIdAsync(1, 1))
                .ReturnsAsync(doorDto);

            // Act
            var result = await _controller.GetDoor(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<DoorDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedDoor = Assert.IsType<DoorDto>(okResult.Value);
            Assert.Equal(1, returnedDoor.DoorId);
            Assert.Equal("Test Door", returnedDoor.DoorName);
        }

        [Fact]
        public async Task GetDoor_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int invalidId = 999;
            _mockDoorService.Setup(service => service.GetDoorByIdAsync(invalidId, 1))
                .ThrowsAsync(new NotFoundException($"Door with ID {invalidId} not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetDoor(invalidId));
        }

        [Fact]
        public async Task CreateDoor_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDoorDto = new CreateDoorDto { DoorName = "New Door", RequiredAccessLevel = 1, OfficeId = 1 };
            var createdDoorDto = new DoorDto { DoorId = 1, DoorName = "New Door", RequiredAccessLevel = 1, OfficeId = 1 };
            _mockDoorService.Setup(service => service.CreateDoorAsync(1, createDoorDto))
                .ReturnsAsync(createdDoorDto);

            // Act
            var result = await _controller.CreateDoor(createDoorDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<DoorDto>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal(nameof(DoorController.GetDoor), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
            var returnedDoor = Assert.IsType<DoorDto>(createdAtActionResult.Value);
            Assert.Equal("New Door", returnedDoor.DoorName);
        }

        [Fact]
        public async Task UpdateDoor_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var updateDoorDto = new UpdateDoorDto { DoorName = "Updated Door", RequiredAccessLevel = 2 };
            _mockDoorService.Setup(service => service.UpdateDoorAsync(1, 1, updateDoorDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateDoor(1, updateDoorDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteDoor_WithValidId_ReturnsNoContent()
        {
            // Arrange
            _mockDoorService.Setup(service => service.DeleteDoorAsync(1, 1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteDoor(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetDoorsByOffice_ReturnsOkResult_WithListOfDoors()
        {
            // Arrange
            var doors = new List<DoorDto>
            {
                new DoorDto { DoorId = 1, DoorName = "Door 1", OfficeId = 1 },
                new DoorDto { DoorId = 2, DoorName = "Door 2", OfficeId = 1 }
            };
            _mockDoorService.Setup(service => service.GetDoorsByOfficeAsync(1, 1))
                .ReturnsAsync(doors);

            // Act
            var result = await _controller.GetDoorsByOffice(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<DoorDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedDoors = Assert.IsType<List<DoorDto>>(okResult.Value);
            Assert.Equal(2, returnedDoors.Count);
        }

        [Fact]
        public async Task GetDoorStatus_ReturnsOkResult_WithDoorStatus()
        {
            // Arrange
            var doorStatus = new DoorStatusDto { DoorId = 1, DoorName = "Test Door", Status = DoorStatusEnum.Locked };
            _mockDoorService.Setup(service => service.GetDoorStatusAsync(1, 1))
                .ReturnsAsync(doorStatus);

            // Act
            var result = await _controller.GetDoorStatus(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<DoorStatusDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedStatus = Assert.IsType<DoorStatusDto>(okResult.Value);
            Assert.Equal(1, returnedStatus.DoorId);
            Assert.Equal(DoorStatusEnum.Locked, returnedStatus.Status);
        }

        [Fact]
        public async Task UnlockDoor_WithValidId_ReturnsOkResult()
        {
            // Arrange
            _mockDoorService.Setup(service => service.UnlockDoorAsync(1, 1))
                .ReturnsAsync("Door unlocked successfully");

            // Act
            var result = await _controller.UnlockDoor(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal("Door unlocked successfully", okResult.Value);
        }

        [Fact]
        public async Task LockDoor_WithValidId_ReturnsOkResult()
        {
            // Arrange
            _mockDoorService.Setup(service => service.LockDoorAsync(1, 1))
                .ReturnsAsync("Door locked successfully");

            // Act
            var result = await _controller.LockDoor(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal("Door locked successfully", okResult.Value);
        }

        [Fact]
        public async Task GrantAccess_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var grantAccessDto = new GrantAccessDto { UserId = 2, DoorId = 1 };
            _mockDoorService.Setup(service => service.GrantAccessAsync(grantAccessDto, 1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.GrantAccess(grantAccessDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Access granted successfully.", okResult.Value);
        }

        [Fact]
        public async Task RevokeAccess_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var revokeAccessDto = new RevokeAccessDto { UserId = 2, DoorId = 1 };
            _mockDoorService.Setup(service => service.RevokeAccessAsync(revokeAccessDto, 1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RevokeAccess(revokeAccessDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Access revoked successfully.", okResult.Value);
        }
    }
}