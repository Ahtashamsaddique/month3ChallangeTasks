using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication2.DTO;
using WebApplication2.Repositries;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IRepoUser _user;

        public AuthController(IConfiguration configuration, IRepoUser user)
        {
            _configuration = configuration;
            _user = user;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] loginDTO loginDto)
        {
            var responce = await _user.loginUser(loginDto);
            if(responce==1)
            {
            // Validate the user credentials (in real-world scenarios, use a user service for validation)
            
                var token = GenerateJwtToken(loginDto.Username);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // Prevent access to cookies via JavaScript
                    Secure = true,   // Set Secure to true in production (HTTPS required)
                    SameSite = SameSiteMode.Strict, // Prevent CSRF attacks
                    Expires = DateTime.Now.AddMinutes(60) // Match token expiration time
                };
                Response.Cookies.Append("AuthToken", token, cookieOptions);
                return Ok(new { Token = token });
            }
            Log.Error("Invalid credentials");
            return Unauthorized("Invalid credentials");
        }

        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
