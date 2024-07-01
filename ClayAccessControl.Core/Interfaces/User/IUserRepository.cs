using ClayAccessControl.Core.Entities;
using System.Threading.Tasks;

namespace ClayAccessControl.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<List<Role>> GetRolesByNamesAsync(List<string> roleNames);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
    }
}