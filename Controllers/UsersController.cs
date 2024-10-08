using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTO;
using WebApplication2.Repositries;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepoUser _repoUser;

        public UsersController(IRepoUser repoUser)
        {
            _repoUser = repoUser;
        }
        [HttpPost]
        public async Task<IActionResult> RegistorUser(RegisterUserDTO registerUserDTO)
        {
            var users= await _repoUser.RegistorUser(registerUserDTO);
            return Ok(users);
        }
    }
}
