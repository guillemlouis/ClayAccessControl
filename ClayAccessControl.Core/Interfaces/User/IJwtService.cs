using System.Collections.Generic;
using System.Threading.Tasks;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Entities;

namespace ClayAccessControl.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user, IEnumerable<string> roles);
    }
}