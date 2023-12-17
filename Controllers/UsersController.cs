using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUsers()
        {
            if (_userManager.Users == null)
            {
                return NotFound();
            }

            // Se sacan los usuarios
            var users = await _userManager.Users.ToListAsync();
            // Se crea una lista de los roles de usuarios
            var userRoles = new List<object>();

            foreach (var usuario in users)
            {
                // Se obtiene el rol de los usuarios
                var rol = await _userManager.GetRolesAsync(usuario);
                // Se añade a la lista de usuarios lo que se va a mostrar, el nombre y el 
                userRoles.Add(new { usuario.UserName, Roles = rol });
            }
            // se devuelve la lista de los usuarios
            return Ok(userRoles);
        }
    }
}
