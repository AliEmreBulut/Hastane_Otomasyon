using Microsoft.AspNetCore.Mvc;
using Hastane_Otomasyon.Data;
using Hastane_Otomasyon.Models;
using System.Linq;

namespace Hastane_Otomasyon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromForm] string username, [FromForm] string password)
        {
            var user = _context.User.FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return Unauthorized("Hatalı kullanıcı adı veya şifre!");

            bool isPasswordValid = false;

            if (user.Password != null && user.Password.StartsWith("$2"))
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            else
            {
                isPasswordValid = (user.Password == password);
            }

            if (!isPasswordValid)
                return Unauthorized("Hatalı kullanıcı adı veya şifre!");

            var role = user.GetRoleName();
            string? target = role switch
            {
                "Patient" => "/sayfalar/hasta-panel.html",
                "Doctor" => "/sayfalar/doktor-panel.html",
                "Admin" => "/sayfalar/admin-panel.html",
                _ => null
            };

            if (target is null)
                return BadRequest("Rol bulunamadı");

            var fullName = user.GetFullName();
            return Ok(new { redirectTo = target, role, fullName, userId = user.UserID });
        }
    }
}