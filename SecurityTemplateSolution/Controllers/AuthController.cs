using Microsoft.AspNetCore.Mvc;
using SecurityTemplateSolution.Models;
using SecurityTemplateSolution.Services;

namespace SecurityTemplateSolution.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SecurityDbContext _context;
        public AuthController(SecurityDbContext context)
        {
            _context = context;
        }

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
       
    }
}
