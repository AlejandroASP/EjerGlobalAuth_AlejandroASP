using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIMusicaAuth_SerafinParedesAlejandro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UsersController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUsers()
        {
            if (_userManager.Users == null)
            {
                return NotFound();
            }

            var users = await _userManager.Users.ToListAsync();

            var userRoles = new List<object>();

            foreach (var usuario in users)
            {
                var rol = await _userManager.GetRolesAsync(usuario);
                userRoles.Add(new { usuario.UserName, Roles = rol });
            }
            return Ok(userRoles);
        }
    }
}
