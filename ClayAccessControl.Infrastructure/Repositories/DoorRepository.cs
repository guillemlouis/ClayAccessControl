using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.Entities;
using ClayAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClayAccessControl.Infrastructure.Repositories
{
    public class DoorRepository : IDoorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Door>> GetAllDoorsAsync()
        {
            return await _context.Doors.Include(d => d.Office).ToListAsync();
        }

        public async Task<Door> GetDoorByIdAsync(int id)
        {
            return await _context.Doors.Include(d => d.Office).FirstOrDefaultAsync(d => d.DoorId == id);
        }

        public async Task<Door> CreateDoorAsync(Door door)
        {
            _context.Doors.Add(door);
            await _context.SaveChangesAsync();
            return door;
        }

        public async Task UpdateDoorAsync(Door door)
        {
            _context.Entry(door).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoorAsync(Door door)
        {
            _context.Doors.Remove(door);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Door>> GetDoorsByOfficeAsync(int officeId)
        {
            return await _context.Doors.Where(d => d.OfficeId == officeId).ToListAsync();
        }

        public async Task<bool> DoorExistsAsync(int id)
        {
            return await _context.Doors.AnyAsync(d => d.DoorId == id);
        }

        public async Task<User> GetUserWithRolesAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> UserHasSpecificAccessAsync(int userId, int doorId)
        {
            return await _context.UserDoorAccesses
                .AnyAsync(uda => uda.UserId == userId && uda.DoorId == doorId && uda.HasAccess);
        }

        public async Task<UserDoorAccess> GetUserDoorAccessAsync(int userId, int doorId)
        {
            return await _context.UserDoorAccesses
                .FirstOrDefaultAsync(uda => uda.UserId == userId && uda.DoorId == doorId);
        }

        public async Task AddUserDoorAccessAsync(UserDoorAccess userDoorAccess)
        {
            _context.UserDoorAccesses.Add(userDoorAccess);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByOfficeAsync(int officeId)
        {
            return await _context.Users.Where(u => u.OfficeId == officeId).ToListAsync();
        }

        public async Task UpdateUserDoorAccessAsync(UserDoorAccess userDoorAccess)
        {
            _context.Entry(userDoorAccess).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}