using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Exceptions;
using ClayAccessControl.API.Models;
using ClayAccessControl.API.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClayAccessControl.API.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    [ApiController]
    [Route("api/[controller]")]
    [UserIdFilter]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventService eventService, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventLogs([FromQuery] EventLogQueryParams queryParams)
        {
            var result = await _eventService.GetEventLogsAsync(UserId, queryParams);
            return this.ApiOk(result, "Event logs retrieved successfully");
        }

        // Property to get UserId set by the UserIdFilter
        private int UserId => (int)HttpContext.Items["UserId"];
    }
}