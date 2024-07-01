using Xunit;
using Moq;
using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Entities;
using ClayAccessControl.Core.Exceptions;
using ClayAccessControl.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClayAccessControl.Tests.Services
{
    public class DoorServiceTests
    {
        private readonly Mock<IDoorRepository> _mockDoorRepository;
        private readonly Mock<IEventService> _mockEventService;
        private readonly Mock<ILogger<DoorService>> _mockLogger;
        private readonly DoorService _doorService;

        public DoorServiceTests()
        {
            _mockDoorRepository = new Mock<IDoorRepository>();
            _mockEventService = new Mock<IEventService>();
            _mockLogger = new Mock<ILogger<DoorService>>();
            _doorService = new DoorService(_mockDoorRepository.Object, _mockEventService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllDoorsAsync_ShouldReturnAllDoors_WhenUserIsAdmin()
        {
            // Arrange
            var adminUser = new User { UserId = 1, Username = "Admin", UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "Admin" } } } };
            var doors = new List<Door>
            {
                new Door { DoorId = 1, DoorName = "Door 1", RequiredAccessLevel = 1, OfficeId = 1 },
                new Door { DoorId = 2, DoorName = "Door 2", RequiredAccessLevel = 2, OfficeId = 2 }
            };
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(adminUser);
            _mockDoorRepository.Setup(repo => repo.GetAllDoorsAsync()).ReturnsAsync(doors);

            // Act
            var result = await _doorService.GetAllDoorsAsync(1);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Door 1", result.First().DoorName);
            Assert.Equal("Door 2", result.Last().DoorName);
        }

        [Fact]
        public async Task GetAllDoorsAsync_ShouldReturnOfficeDoorsOnly_WhenUserIsNotAdmin()
        {
            // Arrange
            var regularUser = new User { UserId = 1, Username = "Regular", OfficeId = 1, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User" } } } };
            var doors = new List<Door>
            {
                new Door { DoorId = 1, DoorName = "Door 1", RequiredAccessLevel = 1, OfficeId = 1 },
                new Door { DoorId = 2, DoorName = "Door 2", RequiredAccessLevel = 1, OfficeId = 1 }
            };
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(regularUser);
            _mockDoorRepository.Setup(repo => repo.GetDoorsByOfficeAsync(1)).ReturnsAsync(doors);

            // Act
            var result = await _doorService.GetAllDoorsAsync(1);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, door => Assert.Equal(1, door.OfficeId));
        }

        [Fact]
        public async Task GetDoorByIdAsync_ShouldReturnDoor_WhenUserHasAccess()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "User", OfficeId = 1, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User", AccessLevel = 1 } } } };
            var door = new Door { DoorId = 1, DoorName = "Test Door", RequiredAccessLevel = 1, OfficeId = 1 };
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);

            // Act
            var result = await _doorService.GetDoorByIdAsync(1, 1);

            // Assert
            Assert.Equal("Test Door", result.DoorName);
        }

        [Fact]
        public async Task GetDoorByIdAsync_ShouldThrowForbiddenException_WhenUserDoesNotHaveAccess()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "User", OfficeId = 2, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User", AccessLevel = 1 } } } };
            var door = new Door { DoorId = 1, DoorName = "Test Door", RequiredAccessLevel = 1, OfficeId = 1 };
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _doorService.GetDoorByIdAsync(1, 1));
        }

        [Fact]
        public async Task CreateDoorAsync_ShouldCreateAndReturnDoor_WhenUserHasPermission()
        {
            // Arrange
            var adminUser = new User { UserId = 1, Username = "Admin", UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "Admin" } } } };
            var createDoorDto = new CreateDoorDto { DoorName = "New Door", RequiredAccessLevel = 1, OfficeId = 1 };
            var createdDoor = new Door { DoorId = 1, DoorName = "New Door", RequiredAccessLevel = 1, OfficeId = 1 };
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(adminUser);
            _mockDoorRepository.Setup(repo => repo.CreateDoorAsync(It.IsAny<Door>())).ReturnsAsync(createdDoor);

            // Act
            var result = await _doorService.CreateDoorAsync(1, createDoorDto);

            // Assert
            Assert.Equal("New Door", result.DoorName);
            Assert.Equal(1, result.DoorId);
        }

        [Fact]
        public async Task CreateDoorAsync_ShouldThrowForbiddenException_WhenUserDoesNotHavePermission()
        {
            // Arrange
            var regularUser = new User { UserId = 1, Username = "Regular", OfficeId = 2, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User" } } } };
            var createDoorDto = new CreateDoorDto { DoorName = "New Door", RequiredAccessLevel = 1, OfficeId = 1 };
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(regularUser);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _doorService.CreateDoorAsync(1, createDoorDto));
        }

        [Fact]
        public async Task UpdateDoorAsync_ShouldUpdateDoor_WhenUserHasAccess()
        {
            // Arrange
            var adminUser = new User { UserId = 1, Username = "Admin", UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "Admin" } } } };
            var door = new Door { DoorId = 1, DoorName = "Old Name", RequiredAccessLevel = 1, OfficeId = 1 };
            var updateDoorDto = new UpdateDoorDto { DoorName = "Updated Name", RequiredAccessLevel = 2 };
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(adminUser);
            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);

            // Act
            await _doorService.UpdateDoorAsync(1, 1, updateDoorDto);

            // Assert
            _mockDoorRepository.Verify(repo => repo.UpdateDoorAsync(It.Is<Door>(d => 
                d.DoorName == "Updated Name" && d.RequiredAccessLevel == 2)), Times.Once);
        }

        [Fact]
        public async Task DeleteDoorAsync_ShouldDeleteDoor_WhenUserHasAccess()
        {
            // Arrange
            var adminUser = new User { UserId = 1, Username = "Admin", UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "Admin" } } } };
            var door = new Door { DoorId = 1, DoorName = "Test Door", RequiredAccessLevel = 1, OfficeId = 1 };
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(adminUser);
            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);

            // Act
            await _doorService.DeleteDoorAsync(1, 1);

            // Assert
            _mockDoorRepository.Verify(repo => repo.DeleteDoorAsync(door), Times.Once);
        }

        [Fact]
        public async Task UnlockDoorAsync_ShouldUnlockDoor_WhenUserHasAccess()
        {
            // Arrange
            var door = new Door { DoorId = 1, DoorName = "Test Door", RequiredAccessLevel = 1, OfficeId = 1, Status = DoorStatusEnum.Locked };
            var user = new User { UserId = 1, Username = "TestUser", OfficeId = 1, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User", AccessLevel = 1 } } } };
            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.UserHasSpecificAccessAsync(1, 1)).ReturnsAsync(true);

            // Act
            var result = await _doorService.UnlockDoorAsync(1, 1);

            // Assert
            Assert.Equal("Door Test Door unlocked successfully.", result);
            Assert.Equal(DoorStatusEnum.Unlocked, door.Status);
            _mockEventService.Verify(service => service.CreateEventAsync(It.IsAny<Event>()), Times.Once);
        }

        [Fact]
        public async Task LockDoorAsync_ShouldLockDoor_WhenUserHasAccess()
        {
            // Arrange
            var door = new Door { DoorId = 1, DoorName = "Test Door", RequiredAccessLevel = 1, OfficeId = 1, Status = DoorStatusEnum.Unlocked };
            var user = new User { UserId = 1, Username = "TestUser", OfficeId = 1, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User", AccessLevel = 1 } } } };
            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.UserHasSpecificAccessAsync(1, 1)).ReturnsAsync(true);

            // Act
            var result = await _doorService.LockDoorAsync(1, 1);

            // Assert
            Assert.Equal("Door Test Door locked successfully.", result);
            Assert.Equal(DoorStatusEnum.Locked, door.Status);
            _mockEventService.Verify(service => service.CreateEventAsync(It.IsAny<Event>()), Times.Once);
        }

        [Fact]
        public async Task GrantAccessAsync_ShouldGrantAccess_WhenUserIsAuthorized()
        {
            // Arrange
            var grantAccessDto = new GrantAccessDto { UserId = 2, DoorId = 1 };
            var door = new Door { DoorId = 1, DoorName = "Test Door", OfficeId = 1 };
            var user = new User { UserId = 2, Username = "TestUser", OfficeId = 1 };
            var adminUser = new User { UserId = 1, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "Admin" } } } };

            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(2)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(adminUser);
            _mockDoorRepository.Setup(repo => repo.GetUserDoorAccessAsync(2, 1)).ReturnsAsync((UserDoorAccess?)null);

            // Act
            await _doorService.GrantAccessAsync(grantAccessDto, 1);

            // Assert
            _mockDoorRepository.Verify(repo => repo.AddUserDoorAccessAsync(It.IsAny<UserDoorAccess>()), Times.Once);
        }

        [Fact]
        public async Task RevokeAccessAsync_ShouldRevokeAccess_WhenUserIsAuthorized()
        {
            // Arrange
            var revokeAccessDto = new RevokeAccessDto { UserId = 2, DoorId = 1 };
            var door = new Door { DoorId = 1, DoorName = "Test Door", OfficeId = 1 };
            var user = new User { UserId = 2, Username = "TestUser", OfficeId = 1 };
            var adminUser = new User { UserId = 1, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "Admin" } } } };
            var existingAccess = new UserDoorAccess { UserId = 2, DoorId = 1, HasAccess = true };

            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(2)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(adminUser);
            _mockDoorRepository.Setup(repo => repo.GetUserDoorAccessAsync(2, 1)).ReturnsAsync(existingAccess);

            // Act
            await _doorService.RevokeAccessAsync(revokeAccessDto, 1);

            // Assert
            Assert.False(existingAccess.HasAccess);
            _mockDoorRepository.Verify(repo => repo.UpdateUserDoorAccessAsync(It.Is<UserDoorAccess>(uda => uda.UserId == 2 && uda.DoorId == 1 && !uda.HasAccess)), Times.Once);
        }

        [Fact]
        public async Task RevokeAccessAsync_ShouldThrowBadRequestException_WhenUserAlreadyDoesntHaveAccess()
        {
            // Arrange
            var revokeAccessDto = new RevokeAccessDto { UserId = 2, DoorId = 1 };
            var door = new Door { DoorId = 1, DoorName = "Test Door", OfficeId = 1 };
            var user = new User { UserId = 2, Username = "TestUser", OfficeId = 1 };
            var adminUser = new User { UserId = 1, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "Admin" } } } };
            var existingAccess = new UserDoorAccess { UserId = 2, DoorId = 1, HasAccess = false };

            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(2)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(adminUser);
            _mockDoorRepository.Setup(repo => repo.GetUserDoorAccessAsync(2, 1)).ReturnsAsync(existingAccess);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _doorService.RevokeAccessAsync(revokeAccessDto, 1));
        }

        [Fact]
        public async Task GetDoorsByOfficeAsync_ShouldReturnDoors_WhenUserIsAuthorized()
        {
            // Arrange
            int officeId = 1;
            int userId = 1;
            var adminUser = new User { UserId = userId, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "Admin" } } } };
            var doors = new List<Door>
            {
                new Door { DoorId = 1, DoorName = "Door 1", RequiredAccessLevel = 1, OfficeId = officeId },
                new Door { DoorId = 2, DoorName = "Door 2", RequiredAccessLevel = 2, OfficeId = officeId }
            };

            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(userId)).ReturnsAsync(adminUser);
            _mockDoorRepository.Setup(repo => repo.GetDoorsByOfficeAsync(officeId)).ReturnsAsync(doors);

            // Act
            var result = await _doorService.GetDoorsByOfficeAsync(officeId, userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Door 1", result.First().DoorName);
            Assert.Equal("Door 2", result.Last().DoorName);
        }

        [Fact]
        public async Task GetDoorsByOfficeAsync_ShouldThrowForbiddenException_WhenUserIsNotAuthorized()
        {
            // Arrange
            int officeId = 1;
            int userId = 1;
            var regularUser = new User
            {
                UserId = userId,
                OfficeId = 2,
                UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User" } } }
            };

            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(userId)).ReturnsAsync(regularUser);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _doorService.GetDoorsByOfficeAsync(officeId, userId));
        }

        [Fact]
        public async Task GetDoorStatusAsync_ShouldReturnDoorStatus_WhenUserHasAccess()
        {
            // Arrange
            int doorId = 1;
            int userId = 1;
            var door = new Door { DoorId = doorId, DoorName = "Test Door", Status = DoorStatusEnum.Locked, OfficeId = 1 };
            var user = new User { UserId = userId, OfficeId = 1, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User", AccessLevel = 1 } } } };

            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(doorId)).ReturnsAsync(door);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(userId)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.UserHasSpecificAccessAsync(userId, doorId)).ReturnsAsync(true);

            // Act
            var result = await _doorService.GetDoorStatusAsync(doorId, userId);

            // Assert
            Assert.Equal(doorId, result.DoorId);
            Assert.Equal("Test Door", result.DoorName);
            Assert.Equal(DoorStatusEnum.Locked, result.Status);
        }

        [Fact]
        public async Task GetDoorStatusAsync_ShouldThrowForbiddenException_WhenUserDoesNotHaveAccess()
        {
            // Arrange
            int doorId = 1;
            int userId = 1;
            var door = new Door { DoorId = doorId, DoorName = "Test Door", Status = DoorStatusEnum.Locked, OfficeId = 1 };
            var user = new User { UserId = userId, OfficeId = 2, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User", AccessLevel = 1 } } } };

            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(doorId)).ReturnsAsync(door);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(userId)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.UserHasSpecificAccessAsync(userId, doorId)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _doorService.GetDoorStatusAsync(doorId, userId));
        }

        [Fact]
        public async Task UnlockDoorAsync_ShouldThrowBadRequestException_WhenDoorIsAlreadyUnlocked()
        {
            // Arrange
            var door = new Door { DoorId = 1, DoorName = "Test Door", RequiredAccessLevel = 1, OfficeId = 1, Status = DoorStatusEnum.Unlocked };
            var user = new User { UserId = 1, Username = "TestUser", OfficeId = 1, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User", AccessLevel = 1 } } } };
            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.UserHasSpecificAccessAsync(1, 1)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _doorService.UnlockDoorAsync(1, 1));
        }

        [Fact]
        public async Task LockDoorAsync_ShouldThrowBadRequestException_WhenDoorIsAlreadyLocked()
        {
            // Arrange
            var door = new Door { DoorId = 1, DoorName = "Test Door", RequiredAccessLevel = 1, OfficeId = 1, Status = DoorStatusEnum.Locked };
            var user = new User { UserId = 1, Username = "TestUser", OfficeId = 1, UserRoles = new List<UserRole> { new UserRole { Role = new Role { RoleName = "User", AccessLevel = 1 } } } };
            _mockDoorRepository.Setup(repo => repo.GetDoorByIdAsync(1)).ReturnsAsync(door);
            _mockDoorRepository.Setup(repo => repo.GetUserWithRolesAsync(1)).ReturnsAsync(user);
            _mockDoorRepository.Setup(repo => repo.UserHasSpecificAccessAsync(1, 1)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _doorService.LockDoorAsync(1, 1));
        }

    }
}