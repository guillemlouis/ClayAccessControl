using ClayAccessControl.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClayAccessControl.Core.Interfaces
{
    public interface IOfficeService
    {
        Task<IEnumerable<OfficeDto>> GetAllOfficesAsync();
        Task<OfficeDto> GetOfficeByIdAsync(int id);
        Task<OfficeDto> CreateOfficeAsync(CreateOfficeDto createOfficeDto);
        Task UpdateOfficeAsync(int id, UpdateOfficeDto updateOfficeDto);
        Task DeleteOfficeAsync(int id);
    }
}