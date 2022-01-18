using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurityTemplateSolution.Models;
using SecurityTemplateSolution.Services;
using System.Linq;

namespace SecurityTemplateSolution.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SecurityDbContext _context;
        public AuthController(SecurityDbContext context)
        {
            _context = context;
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

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users =  _context.Users.Select(u => new {u.Id, u.Login }).ToList();
            return Ok(users);
        }
       
    }
}
