using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Entities;
using ClayAccessControl.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ClayAccessControl.Infrastructure.Services
{
    public class DoorService : IDoorService
    {
        private readonly IDoorRepository _doorRepository;
        private readonly IEventService _eventService;
        private readonly ILogger<DoorService> _logger;

        public DoorService(IDoorRepository doorRepository, IEventService eventService, ILogger<DoorService> logger)
        {
            _doorRepository = doorRepository;
            _eventService = eventService;
            _logger = logger;
        }

        public async Task<IEnumerable<DoorDto>> GetAllDoorsAsync(int userId)
        {
            var user = await GetUserAsync(userId);
            var isAdmin = IsAdmin(user);

            IEnumerable<Door> doors = isAdmin ? await _doorRepository.GetAllDoorsAsync() 
                                              : await _doorRepository.GetDoorsByOfficeAsync(user.OfficeId.Value);

            return doors.Select(ToDoorDto);
        }

        public async Task<DoorDto> GetDoorByIdAsync(int id, int userId)
        {
            var door = await GetDoorAsync(id);
            var user = await GetUserAsync(userId);

            if (!HasAccessToDoor(user, door))
                throw new ForbiddenException("You do not have permission to see door details for this office.");

            return ToDoorDto(door);
        }

        public async Task<DoorDto> CreateDoorAsync(int userId, CreateDoorDto createDoorDto)
        {
            var user = await GetUserAsync(userId);

            if (!HasPermissionToModifyDoor(user, createDoorDto.OfficeId))
                throw new ForbiddenException("You do not have permission to create doors for this office.");

            var door = new Door
            {
                DoorName = createDoorDto.DoorName,
                RequiredAccessLevel = createDoorDto.RequiredAccessLevel,
                OfficeId = createDoorDto.OfficeId
            };

            var createdDoor = await _doorRepository.CreateDoorAsync(door);
            return ToDoorDto(createdDoor);
        }

        public async Task UpdateDoorAsync(int id, int userId, UpdateDoorDto updateDoorDto)
        {
            var door = await GetDoorAsync(id);
            var user = await GetUserAsync(userId);

            if (!HasAccessToDoor(user, door))
                throw new ForbiddenException("You do not have permission to update doors for this office.");

            door.DoorName = updateDoorDto.DoorName;
            door.RequiredAccessLevel = updateDoorDto.RequiredAccessLevel;

            await _doorRepository.UpdateDoorAsync(door);
        }

        public async Task DeleteDoorAsync(int id, int userId)
        {
            var door = await GetDoorAsync(id);
            var user = await GetUserAsync(userId);

            if (!HasAccessToDoor(user, door))
                throw new ForbiddenException("You do not have permission to delete doors for this office.");

            await _doorRepository.DeleteDoorAsync(door);
        }

        public async Task<IEnumerable<DoorDto>> GetDoorsByOfficeAsync(int officeId, int userId)
        {
            var user = await GetUserAsync(userId);

            if (!HasPermissionToModifyDoor(user, officeId))
                throw new ForbiddenException("You do not have permission to view doors for this office.");

            var doors = await _doorRepository.GetDoorsByOfficeAsync(officeId);
            return doors.Select(ToDoorDto);
        }

        public async Task<DoorStatusDto> GetDoorStatusAsync(int id, int userId)
        {
            var door = await GetDoorAsync(id);
            var user = await GetUserAsync(userId);

            if (!await HasAccessToDoorAsync(user, door))
                throw new ForbiddenException($"You do not have permission to check the status of door {door.DoorName}.");

            return new DoorStatusDto
            {
                DoorId = door.DoorId,
                DoorName = door.DoorName,
                Status = door.Status
            };
        }

        public async Task<string> UnlockDoorAsync(int id, int userId)
        {
            var door = await GetDoorAsync(id);
            var user = await GetUserAsync(userId);

            if (!await HasAccessToDoorAsync(user, door))
                throw new ForbiddenException($"You do not have permission to unlock door {door.DoorName}.");

            if (door.Status == DoorStatusEnum.Unlocked)
                throw new BadRequestException($"Door {door.DoorName} is already unlocked.");

            bool unlockSuccessful = SimulateUnlockDoor(door);
            if (!unlockSuccessful)
                throw new Exception($"Failed to unlock door {door.DoorName}. Physical control system error.");

            await LogDoorEventAsync(userId, door, "DoorUnlocked", $"Door {door.DoorName} was unlocked by user {user.Username}");

            door.Status = DoorStatusEnum.Unlocked;
            await _doorRepository.UpdateDoorAsync(door);

            _logger.LogInformation($"Door {door.DoorName} unlocked successfully by user {user.Username}");

            return $"Door {door.DoorName} unlocked successfully.";
        }

        public async Task<string> LockDoorAsync(int id, int userId)
        {
            var door = await GetDoorAsync(id);
            var user = await GetUserAsync(userId);

            if (!await HasAccessToDoorAsync(user, door))
                throw new ForbiddenException($"You do not have permission to lock door {door.DoorName}.");

            if (door.Status == DoorStatusEnum.Locked)
                throw new BadRequestException($"Door {door.DoorName} is already locked.");

            bool lockSuccessful = SimulateLockDoor(door);
            if (!lockSuccessful)
                throw new Exception($"Failed to lock door {door.DoorName}. Physical control system error.");

            await LogDoorEventAsync(userId, door, "DoorLocked", $"Door {door.DoorName} was locked by user {user.Username}");

            door.Status = DoorStatusEnum.Locked;
            await _doorRepository.UpdateDoorAsync(door);

            _logger.LogInformation($"Door {door.DoorName} locked successfully by user {user.Username}");

            return $"Door {door.DoorName} locked successfully.";
        }

        public async Task GrantAccessAsync(GrantAccessDto grantAccessDto, int currentUserId)
        {
            var door = await GetDoorAsync(grantAccessDto.DoorId);
            var user = await GetUserAsync(grantAccessDto.UserId);
            var currentUser = await GetUserAsync(currentUserId);

            if (!HasPermissionToGrantAccess(currentUser, door))
                throw new ForbiddenException("You do not have permission to grant access to this door.");

            if (user.OfficeId != door.OfficeId)
                throw new BadRequestException("Cannot grant access to a user from a different office.");

            var existingAccess = await _doorRepository.GetUserDoorAccessAsync(grantAccessDto.UserId, grantAccessDto.DoorId);

            if (existingAccess != null)
            {
                existingAccess.HasAccess = true;
                await _doorRepository.UpdateUserDoorAccessAsync(existingAccess);
            }
            else
            {
                await _doorRepository.AddUserDoorAccessAsync(new UserDoorAccess
                {
                    UserId = grantAccessDto.UserId,
                    DoorId = grantAccessDto.DoorId,
                    HasAccess = true
                });
            }

            _logger.LogInformation($"Access granted to user {user.Username} for door {door.DoorName} by user {currentUser.Username}");
        }

        public async Task RevokeAccessAsync(RevokeAccessDto revokeAccessDto, int currentUserId)
        {
            var door = await GetDoorAsync(revokeAccessDto.DoorId);
            var user = await GetUserAsync(revokeAccessDto.UserId);
            var currentUser = await GetUserAsync(currentUserId);

            if (!HasPermissionToGrantAccess(currentUser, door))
                throw new ForbiddenException("You do not have permission to revoke access for this door.");

            if (user.OfficeId != door.OfficeId)
                throw new BadRequestException("Cannot revoke access for a user from a different office.");

            var existingAccess = await _doorRepository.GetUserDoorAccessAsync(revokeAccessDto.UserId, revokeAccessDto.DoorId);

            if (existingAccess != null)
            {
                if (existingAccess.HasAccess)
                {
                    existingAccess.HasAccess = false;
                    await _doorRepository.UpdateUserDoorAccessAsync(existingAccess);
                    _logger.LogInformation($"Access revoked for user {user.Username} to door {door.DoorName} by user {currentUser.Username}");
                }
                else
                {
                    throw new BadRequestException("User already doesn't have access to this door.");
                }
            }
            else
            {
                throw new NotFoundException("No access record found for this user and door.");
            }
        }

        private async Task<User> GetUserAsync(int userId)
        {
            var user = await _doorRepository.GetUserWithRolesAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found.");
            return user;
        }

        private async Task<Door> GetDoorAsync(int doorId)
        {
            var door = await _doorRepository.GetDoorByIdAsync(doorId);
            if (door == null)
                throw new NotFoundException($"Door with ID {doorId} not found.");
            return door;
        }

        private async Task LogDoorEventAsync(int userId, Door door, string eventType, string description)
        {
            var doorEvent = new Event
            {
                UserId = userId,
                DoorId = door.DoorId,
                EventType = eventType,
                Timestamp = DateTime.UtcNow,
                Description = description
            };

            await _eventService.CreateEventAsync(doorEvent);
        }

        private bool HasAccessToDoor(User user, Door door)
        {
            return IsAdmin(user) || (user.OfficeId == door.OfficeId && user.UserRoles.Any(ur => ur.Role.AccessLevel >= door.RequiredAccessLevel));
        }

        private async Task<bool> HasAccessToDoorAsync(User user, Door door)
        {
            if (HasAccessToDoor(user, door))
                return true;

            return await _doorRepository.UserHasSpecificAccessAsync(user.UserId, door.DoorId);
        }

        private bool HasPermissionToModifyDoor(User user, int officeId)
        {
            return IsAdmin(user) || (IsManager(user) && user.OfficeId == officeId);
        }

        private bool HasPermissionToGrantAccess(User currentUser, Door door)
        {
            return IsAdmin(currentUser) || (IsManager(currentUser) && currentUser.OfficeId == door.OfficeId);
        }

        private bool IsAdmin(User user) => user.UserRoles.Any(ur => ur.Role.RoleName == "Admin");

        private bool IsManager(User user) => user.UserRoles.Any(ur => ur.Role.RoleName == "Manager");

        private bool SimulateUnlockDoor(Door door)
        {
            _logger.LogInformation($"Simulating unlock operation for door {door.DoorName}");
            return true;
        }

        private bool SimulateLockDoor(Door door)
        {
            _logger.LogInformation($"Simulating lock operation for door {door.DoorName}");
            return true;
        }

        private DoorDto ToDoorDto(Door door)
        {
            return new DoorDto
            {
                DoorId = door.DoorId,
                DoorName = door.DoorName,
                RequiredAccessLevel = door.RequiredAccessLevel,
                OfficeId = door.OfficeId
            };
        }
    }
}