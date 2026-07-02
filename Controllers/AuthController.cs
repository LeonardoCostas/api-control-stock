using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApipractica.models;

namespace WebApipractica.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto dto)
        {
            var username = _configuration["AdminUser:Username"] ?? "admin";
            var password = _configuration["AdminUser:Password"] ?? "zamponi2026";

            if (!string.Equals(dto.Username, username, StringComparison.OrdinalIgnoreCase) || dto.Password != password)
                return Unauthorized("Usuario o contrasena incorrectos.");

            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(8);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = expires,
                Username = username
            });
        }
    }
}
