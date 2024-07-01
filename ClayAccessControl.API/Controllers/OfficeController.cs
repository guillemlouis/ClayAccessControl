using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClayAccessControl.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class OfficeController : ControllerBase
    {
        private readonly IOfficeService _officeService;

        public OfficeController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OfficeDto>>> GetOffices()
        {
            var offices = await _officeService.GetAllOfficesAsync();
            return Ok(offices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OfficeDto>> GetOffice(int id)
        {
            var office = await _officeService.GetOfficeByIdAsync(id);
            return Ok(office);
        }

        [HttpPost]
        public async Task<ActionResult<OfficeDto>> CreateOffice(CreateOfficeDto createOfficeDto)
        {
            var createdOffice = await _officeService.CreateOfficeAsync(createOfficeDto);
            return CreatedAtAction(nameof(GetOffice), new { id = createdOffice.OfficeId }, createdOffice);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOffice(int id, UpdateOfficeDto updateOfficeDto)
        {
            await _officeService.UpdateOfficeAsync(id, updateOfficeDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffice(int id)
        {
            await _officeService.DeleteOfficeAsync(id);
            return NoContent();
        }
    }
}