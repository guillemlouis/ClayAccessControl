using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Exceptions;
using ClayAccessControl.API.Models;
using ClayAccessControl.API.Filters;
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
        public async Task<IActionResult> GetDoors()
        {
            var doors = await _doorService.GetAllDoorsAsync(UserId);
            return this.ApiOk(doors, "Doors retrieved successfully");
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetDoor(int id)
        {
            var door = await _doorService.GetDoorByIdAsync(id, UserId);
            return this.ApiOk(door, "Door retrieved successfully");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateDoor(CreateDoorDto createDoorDto)
        {
            var createdDoor = await _doorService.CreateDoorAsync(UserId, createDoorDto);
            return this.ApiCreated(createdDoor, "Door created successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateDoor(int id, UpdateDoorDto updateDoorDto)
        {
            await _doorService.UpdateDoorAsync(id, UserId, updateDoorDto);
            return this.ApiOk<object>(null, "Door updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteDoor(int id)
        {
            await _doorService.DeleteDoorAsync(id, UserId);
            return this.ApiOk<object>(null, "Door deleted successfully");
        }

        [HttpGet("ByOffice/{officeId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetDoorsByOffice(int officeId)
        {
            var doors = await _doorService.GetDoorsByOfficeAsync(officeId, UserId);
            return this.ApiOk(doors, "Doors retrieved successfully");
        }

        [HttpGet("{id}/Status")]
        public async Task<IActionResult> GetDoorStatus(int id)
        {
            var status = await _doorService.GetDoorStatusAsync(id, UserId);
            return this.ApiOk(status, "Door status retrieved successfully");
        }

        [HttpPost("{id}/Unlock")]
        public async Task<IActionResult> UnlockDoor(int id)
        {
            var result = await _doorService.UnlockDoorAsync(id, UserId);
            return this.ApiOk(result, "Door unlock operation completed");
        }

        [HttpPost("{id}/Lock")]
        public async Task<IActionResult> LockDoor(int id)
        {
            var result = await _doorService.LockDoorAsync(id, UserId);
            return this.ApiOk(result, "Door lock operation completed");
        }

        [HttpPost("GrantAccess")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GrantAccess([FromBody] GrantAccessDto grantAccessDto)
        {
            await _doorService.GrantAccessAsync(grantAccessDto, UserId);
            return this.ApiOk<object>(null, "Access granted successfully");

        }

        [HttpPost("RevokeAccess")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RevokeAccess([FromBody] RevokeAccessDto revokeAccessDto)
        {
            await _doorService.RevokeAccessAsync(revokeAccessDto, UserId);
            return this.ApiOk<object>(null, "Access revoked successfully");
        }

        // Property to get UserId set by the UserIdFilter
        private int UserId => (int)HttpContext.Items["UserId"];
    }

    
}