using ClayAccessControl.Core.Entities;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace ClayAccessControl.Core.Interfaces
{
    public interface IEventRepository
    {
        IQueryable<Event> GetEventsQuery();
        Task<User> GetUserWithRolesAsync(int userId);
        Task<int> CountEventsAsync(IQueryable<Event> query);
        Task<List<Event>> GetPaginatedEventsAsync(IQueryable<Event> query, int skip, int take);
        Task<Event> CreateEventAsync(Event eventToCreate);
    }
}