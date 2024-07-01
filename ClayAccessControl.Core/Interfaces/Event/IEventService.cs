using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Entities;
using System.Threading.Tasks;

namespace ClayAccessControl.Core.Interfaces
{
    public interface IEventService
    {
        Task<PagedResult<EventLogDto>> GetEventLogsAsync(int currentUserId, EventLogQueryParams queryParams);
        Task CreateEventAsync(Event eventToCreate);
    }
}