using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Exceptions;
using ClayAccessControl.API.Models;
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
        public async Task<IActionResult> GetOffices()
        {
            var offices = await _officeService.GetAllOfficesAsync();
            return this.ApiOk(offices, "Offices retrieved successfully");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOffice(int id)
        {
            var office = await _officeService.GetOfficeByIdAsync(id);
            return this.ApiOk(office, "Office retrieved successfully");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOffice(CreateOfficeDto createOfficeDto)
        {
            var createdOffice = await _officeService.CreateOfficeAsync(createOfficeDto);
            return this.ApiCreated(createdOffice, "Office created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOffice(int id, UpdateOfficeDto updateOfficeDto)
        {
            await _officeService.UpdateOfficeAsync(id, updateOfficeDto);
            return this.ApiOk<object>(null, "Office updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffice(int id)
        {
            await _officeService.DeleteOfficeAsync(id);
            return this.ApiOk<object>(null, "Office deleted successfully");
        }
    }
}