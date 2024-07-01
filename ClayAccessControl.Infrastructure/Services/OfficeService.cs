using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Entities;
using ClayAccessControl.Core.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClayAccessControl.Infrastructure.Services
{
    public class OfficeService : IOfficeService
    {
        private readonly IOfficeRepository _officeRepository;

        public OfficeService(IOfficeRepository officeRepository)
        {
            _officeRepository = officeRepository;
        }

        public async Task<IEnumerable<OfficeDto>> GetAllOfficesAsync()
        {
            var offices = await _officeRepository.GetAllOfficesAsync();
            return offices.Select(o => new OfficeDto
            {
                OfficeId = o.OfficeId,
                OfficeName = o.OfficeName,
                Address = o.Address
            });
        }

        public async Task<OfficeDto> GetOfficeByIdAsync(int id)
        {
            var office = await _officeRepository.GetOfficeByIdAsync(id);
            if (office == null)
            {
                throw new NotFoundException($"Office with ID {id} not found.");
            }

            return new OfficeDto
            {
                OfficeId = office.OfficeId,
                OfficeName = office.OfficeName,
                Address = office.Address
            };
        }

        public async Task<OfficeDto> CreateOfficeAsync(CreateOfficeDto createOfficeDto)
        {
            var office = new Office
            {
                OfficeName = createOfficeDto.OfficeName,
                Address = createOfficeDto.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdOffice = await _officeRepository.CreateOfficeAsync(office);

            return new OfficeDto
            {
                OfficeId = createdOffice.OfficeId,
                OfficeName = createdOffice.OfficeName,
                Address = createdOffice.Address
            };
        }

        public async Task UpdateOfficeAsync(int id, UpdateOfficeDto updateOfficeDto)
        {
            var office = await _officeRepository.GetOfficeByIdAsync(id);
            if (office == null)
            {
                throw new NotFoundException($"Office with ID {id} not found.");
            }

            office.OfficeName = updateOfficeDto.OfficeName;
            office.Address = updateOfficeDto.Address;
            office.UpdatedAt = DateTime.UtcNow;

            await _officeRepository.UpdateOfficeAsync(office);
        }

        public async Task DeleteOfficeAsync(int id)
        {
            var office = await _officeRepository.GetOfficeByIdAsync(id);
            if (office == null)
            {
                throw new NotFoundException($"Office with ID {id} not found.");
            }

            await _officeRepository.DeleteOfficeAsync(office);
        }
    }
}