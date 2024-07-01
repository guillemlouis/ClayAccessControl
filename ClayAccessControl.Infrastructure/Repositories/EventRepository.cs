using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.Entities;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ClayAccessControl.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Event> GetEventsQuery()
        {
            return _context.Events
                .Include(e => e.User)
                .Include(e => e.Door)
                .ThenInclude(d => d.Office);
        }

        public async Task<Event> CreateEventAsync(Event eventToCreate)
        {
            _context.Events.Add(eventToCreate);
            await _context.SaveChangesAsync();
            return eventToCreate;
        }

        public async Task<User> GetUserWithRolesAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<int> CountEventsAsync(IQueryable<Event> query)
        {
            return await query.CountAsync();
        }

        public async Task<List<Event>> GetPaginatedEventsAsync(IQueryable<Event> query, int skip, int take)
        {
            return await query
                .OrderByDescending(e => e.Timestamp)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}