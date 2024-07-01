using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Core.Entities;
using ClayAccessControl.Core.Exceptions;
using System.Threading.Tasks;

namespace ClayAccessControl.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordService _passwordService;

        public AuthService(IUserRepository userRepository, PasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task<(User user, List<string> roles)> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null || !_passwordService.VerifyPassword(password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid username or password.");
            }

            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
            return (user, roles);
        }

        public async Task<User> RegisterUserAsync(RegisterDto model)
        {
            if (await _userRepository.UsernameExistsAsync(model.Username))
            {
                throw new ConflictException("Username already exists.");
            }

            var roles = await _userRepository.GetRolesByNamesAsync(model.Roles);
            if (roles.Count != model.Roles.Count)
            {
                throw new BadRequestException("One or more specified roles do not exist.");
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = _passwordService.HashPassword(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                OfficeId = model.OfficeId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserRoles = roles.Select(r => new UserRole { Role = r }).ToList()
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }
    }
}