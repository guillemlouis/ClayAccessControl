using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Entities;
using ClayAccessControl.Core.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace ClayAccessControl.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task CreateEventAsync(Event eventToCreate)
        {
            await _eventRepository.CreateEventAsync(eventToCreate);
        }

        public async Task<PagedResult<EventLogDto>> GetEventLogsAsync(int currentUserId, EventLogQueryParams queryParams)
        {
            var currentUser = await _eventRepository.GetUserWithRolesAsync(currentUserId);
            if (currentUser == null)
            {
                throw new UnauthorizedException("Current user not found.");
            }

            var isAdmin = currentUser.UserRoles.Any(ur => ur.Role.RoleName == "Admin");

            var query = _eventRepository.GetEventsQuery();

            // Apply filters
            query = ApplyFilters(query, queryParams, currentUser, isAdmin);

            // Get total count
            var totalCount = await _eventRepository.CountEventsAsync(query);

            // Apply pagination
            int pageSize = queryParams.PageSize ?? 10;
            int pageNumber = queryParams.PageNumber ?? 1;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var eventsList = await _eventRepository.GetPaginatedEventsAsync(query, (pageNumber - 1) * pageSize, pageSize);

            var events = eventsList.Select(e => new EventLogDto
            {
                EventId = e.EventId,
                Timestamp = e.Timestamp,
                EventType = e.EventType ?? string.Empty,
                Description = e.Description ?? string.Empty,
                UserName = e.User?.Username ?? string.Empty,
                DoorName = e.Door?.DoorName ?? string.Empty,
                OfficeName = e.Door?.Office?.OfficeName ?? string.Empty
            }).ToList();

            return new PagedResult<EventLogDto>
            {
                Items = events,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        private IQueryable<Event> ApplyFilters(IQueryable<Event> query, EventLogQueryParams queryParams, User currentUser, bool isAdmin)
        {
            if (queryParams.OfficeId.HasValue)
            {
                if (!isAdmin && currentUser.OfficeId != queryParams.OfficeId)
                {
                    throw new ForbiddenException("You do not have permission to view events for this office.");
                }
                query = query.Where(e => e.Door.Office.OfficeId == queryParams.OfficeId);
            }
            else if (!isAdmin)
            {
                query = query.Where(e => e.Door.Office.OfficeId == currentUser.OfficeId);
            }

            if (queryParams.StartDate.HasValue)
            {
                query = query.Where(e => e.Timestamp >= queryParams.StartDate.Value);
            }

            if (queryParams.EndDate.HasValue)
            {
                query = query.Where(e => e.Timestamp <= queryParams.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(queryParams.EventType))
            {
                query = query.Where(e => e.EventType == queryParams.EventType);
            }

            if (queryParams.UserId.HasValue)
            {
                query = query.Where(e => e.UserId == queryParams.UserId.Value);
            }

            if (queryParams.DoorId.HasValue)
            {
                query = query.Where(e => e.DoorId == queryParams.DoorId.Value);
            }

            return query;
        }
    }
}