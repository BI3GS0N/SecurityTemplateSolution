using Microsoft.AspNetCore.Mvc;
using SecurityTemplateSolution.Models;

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

        [HttpPost]
        public IActionResult Register([FromBody]AuthRequest request)
        {
            var user = new User
            {
                Login = request.Login
            };

            _context.Add(user);
            _context.SaveChanges();
            return Ok(new { user.Id, user.Login });
        }
       
    }
}
