using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Entities;
using System.Threading.Tasks;

namespace ClayAccessControl.Core.Interfaces
{
    public interface IAuthService
    {
        Task<(User user, List<string> roles)> AuthenticateAsync(string username, string password);
        Task<User> RegisterUserAsync(RegisterDto model);
    }
}