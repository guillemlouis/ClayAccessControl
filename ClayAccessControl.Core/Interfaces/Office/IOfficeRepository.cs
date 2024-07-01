using ClayAccessControl.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClayAccessControl.Core.Interfaces
{
    public interface IOfficeRepository
    {
        Task<IEnumerable<Office>> GetAllOfficesAsync();
        Task<Office> GetOfficeByIdAsync(int id);
        Task<Office> CreateOfficeAsync(Office office);
        Task UpdateOfficeAsync(Office office);
        Task DeleteOfficeAsync(Office office);
    }
}