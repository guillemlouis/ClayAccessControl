using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Exceptions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClayAccessControl.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [UserIdFilter]
    public class DoorController : ControllerBase
    {
        private readonly IDoorService _doorService;
        private readonly ILogger<DoorController> _logger;

        public DoorController(IDoorService doorService, ILogger<DoorController> logger)
        {
            _doorService = doorService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<DoorDto>>> GetDoors()
        {
            var doors = await _doorService.GetAllDoorsAsync(UserId);
            return Ok(doors);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<DoorDto>> GetDoor(int id)
        {
            var door = await _doorService.GetDoorByIdAsync(id, UserId);
            return Ok(door);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<DoorDto>> CreateDoor(CreateDoorDto createDoorDto)
        {
            var createdDoor = await _doorService.CreateDoorAsync(UserId, createDoorDto);
            return CreatedAtAction(nameof(GetDoor), new { id = createdDoor.DoorId }, createdDoor);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateDoor(int id, UpdateDoorDto updateDoorDto)
        {
            await _doorService.UpdateDoorAsync(id, UserId, updateDoorDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteDoor(int id)
        {
            await _doorService.DeleteDoorAsync(id, UserId);
            return NoContent();
        }

        [HttpGet("ByOffice/{officeId}")]
        public async Task<ActionResult<IEnumerable<DoorDto>>> GetDoorsByOffice(int officeId)
        {
            var doors = await _doorService.GetDoorsByOfficeAsync(officeId, UserId);
            return Ok(doors);
        }

        [HttpGet("{id}/Status")]
        public async Task<ActionResult<DoorStatusDto>> GetDoorStatus(int id)
        {
            var status = await _doorService.GetDoorStatusAsync(id, UserId);
            return Ok(status);
        }

        [HttpPost("{id}/Unlock")]
        public async Task<ActionResult<string>> UnlockDoor(int id)
        {
            var result = await _doorService.UnlockDoorAsync(id, UserId);
            return Ok(result);
        }

        [HttpPost("{id}/Lock")]
        public async Task<ActionResult<string>> LockDoor(int id)
        {
            var result = await _doorService.LockDoorAsync(id, UserId);
            return Ok(result);
        }

        [HttpPost("GrantAccess")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GrantAccess([FromBody] GrantAccessDto grantAccessDto)
        {
            await _doorService.GrantAccessAsync(grantAccessDto, UserId);
            return Ok("Access granted successfully.");
        }

        [HttpPost("RevokeAccess")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RevokeAccess([FromBody] RevokeAccessDto revokeAccessDto)
        {
            await _doorService.RevokeAccessAsync(revokeAccessDto, UserId);
            return Ok("Access revoked successfully.");
        }

        // Property to get UserId set by the UserIdFilter
        private int UserId => (int)HttpContext.Items["UserId"];
    }

    public class UserIdFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userId, out int parsedUserId))
            {
                context.HttpContext.Items["UserId"] = parsedUserId;
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
            base.OnActionExecuting(context);
        }
    }
}