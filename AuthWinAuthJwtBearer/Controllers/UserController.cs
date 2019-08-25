using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthJwtBearer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        public UserController(IConfiguration config)
        {
            this._config = config;

        }

        [Authorize(AuthenticationSchemes = "Windows")]
        [HttpGet("login")]
        public IActionResult login()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "Pablo"),
                new Claim(ClaimTypes.Name, "Pablo Rivas"),
                new Claim(ClaimTypes.Role, "User"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creeds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = creeds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("protected")]
        public IActionResult secret()
        {
            var userName = User.Identity.Name;

            var claims = User.Claims.Select(x => new { x.Type, x.Value });

            return Ok(new { userName, claims });
        }

    }
}
