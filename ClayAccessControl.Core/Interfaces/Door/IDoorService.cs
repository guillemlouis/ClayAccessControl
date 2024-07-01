using ClayAccessControl.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClayAccessControl.Core.Interfaces
{
    public interface IDoorService
    {
        Task<IEnumerable<DoorDto>> GetAllDoorsAsync(int userId);
        Task<DoorDto> GetDoorByIdAsync(int id, int userId);
        Task<DoorDto> CreateDoorAsync(int userId, CreateDoorDto createDoorDto);
        Task UpdateDoorAsync(int id, int userId, UpdateDoorDto updateDoorDto);
        Task DeleteDoorAsync(int id, int userId);
        Task<IEnumerable<DoorDto>> GetDoorsByOfficeAsync(int officeId, int userId);
        Task<DoorStatusDto> GetDoorStatusAsync(int id, int userId);
        Task<string> UnlockDoorAsync(int id, int userId);
        Task<string> LockDoorAsync(int id, int userId);
        Task GrantAccessAsync(GrantAccessDto grantAccessDto, int currentUserId);
        Task RevokeAccessAsync(RevokeAccessDto revokeAccessDto, int currentUserId);
    }
}