using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Users.V1.Models;
using WebApi.Controllers.Users.V1.Services;

namespace WebApi.Controllers.Users.V1
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterModel model)
        {
            var response = await _userService.Register(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response.Message);
        }

        // POST: api/users/authenticate
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] UserAuthenticateModel model)
        {
            var response = await _userService.Authenticate(model);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response.Message);
        }

        // GET: api/users/profile
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            var username = User.Identity.Name;

            // Use the username to fetch the user details from your service
            var user = await _userService.GetUserByUsername(username);

            if (user != null)
            {
                return Ok(user);
            }


            return NotFound("User not found.");
        }
    }
}