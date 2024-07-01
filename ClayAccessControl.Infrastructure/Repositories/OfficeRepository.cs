using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.Entities;
using ClayAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClayAccessControl.Infrastructure.Repositories
{
    public class OfficeRepository : IOfficeRepository
    {
        private readonly ApplicationDbContext _context;

        public OfficeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Office>> GetAllOfficesAsync()
        {
            return await _context.Offices.ToListAsync();
        }

        public async Task<Office> GetOfficeByIdAsync(int id)
        {
            return await _context.Offices.FindAsync(id);
        }

        public async Task<Office> CreateOfficeAsync(Office office)
        {
            _context.Offices.Add(office);
            await _context.SaveChangesAsync();
            return office;
        }

        public async Task UpdateOfficeAsync(Office office)
        {
            _context.Entry(office).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOfficeAsync(Office office)
        {
            _context.Offices.Remove(office);
            await _context.SaveChangesAsync();
        }
    }
}