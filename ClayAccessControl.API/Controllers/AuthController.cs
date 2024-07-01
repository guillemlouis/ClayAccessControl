using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClayAccessControl.Core.Interfaces;
using ClayAccessControl.Core.DTOs;
using ClayAccessControl.Infrastructure.Services;
using ClayAccessControl.Core.Exceptions;
using System.Threading.Tasks;

namespace ClayAccessControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(IAuthService authService, JwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var (user, roles) = await _authService.AuthenticateAsync(model.Username, model.Password);
            var token = _jwtService.GenerateToken(user, roles);
            return Ok(new { Token = token, Roles = roles });
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = await _authService.RegisterUserAsync(model);
            return StatusCode(201, new { Message = "User registered successfully", UserId = user.UserId });
        }
    }
}