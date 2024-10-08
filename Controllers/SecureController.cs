using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecureController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok("This is a public endpoint, no authentication required.");
        }

        [Authorize]
        [HttpGet("private")]
        public IActionResult PrivateEndpoint()
        {
            return Ok("This is a private endpoint, accessible only to authenticated users.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminEndpoint()
        {
            return Ok("This endpoint is accessible only to Admin users.");
        }
    }
}
