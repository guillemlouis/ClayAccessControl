using ClayAccessControl.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClayAccessControl.Core.Interfaces
{
    public interface IDoorRepository
    {
        Task<IEnumerable<Door>> GetAllDoorsAsync();
        Task<Door> GetDoorByIdAsync(int id);
        Task<Door> CreateDoorAsync(Door door);
        Task UpdateDoorAsync(Door door);
        Task DeleteDoorAsync(Door door);
        Task<IEnumerable<Door>> GetDoorsByOfficeAsync(int officeId);
        Task<bool> DoorExistsAsync(int id);
        Task<User> GetUserWithRolesAsync(int userId);
        Task<bool> UserHasSpecificAccessAsync(int userId, int doorId);
        Task<UserDoorAccess> GetUserDoorAccessAsync(int userId, int doorId);
        Task AddUserDoorAccessAsync(UserDoorAccess userDoorAccess);
        Task<IEnumerable<User>> GetUsersByOfficeAsync(int officeId);
        Task UpdateUserDoorAccessAsync(UserDoorAccess userDoorAccess);
    }
}