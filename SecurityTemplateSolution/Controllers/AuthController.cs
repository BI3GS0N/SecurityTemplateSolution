using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecurityTemplateSolution.Models;
using SecurityTemplateSolution.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SecurityTemplateSolution.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SecurityDbContext _context;
        private readonly TokenSettings _tokenSettings;

        public AuthController(SecurityDbContext context, IOptions<TokenSettings> tokenSettings)
        {
            _context = context;
            _tokenSettings = tokenSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]AuthRequest request)
        {
            var user = new User
            {
                Login = request.Login
            };

            AuthService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Add(user);
            _context.SaveChanges();
            return Ok(new { user.Id, user.Login });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == request.Login);
            if (user == null)
                BadRequest("Nieprawidłowa nazwa użytkownika");

            if (AuthService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                BadRequest("Nieprawidłowe hasło");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users =  _context.Users.Select(u => new {u.Id, u.Login }).ToList();
            return Ok(users);
        }
       
    }
}
