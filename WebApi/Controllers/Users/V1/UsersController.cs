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

        /// <summary>
        /// Registers a new user with the provided user details.
        /// </summary>
        /// <param name="model">The user registration details, including username, email, and password.</param>
        /// <returns>A <see cref="RegisterResponse"/> object containing the registration result. 
        /// Success is true if registration is successful, along with user details. 
        /// If unsuccessful, Success is false with an error message.</returns>
        /// <remarks>
        /// This method checks for existing users with the same username or email to prevent duplicates.
        /// It hashes the password for secure storage and logs the process at various stages.
        /// Exceptions during registration are caught and logged, returning a generic error message to the caller.
        /// </remarks>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterModel model)
        {
            var response = await _userService.Register(model);
            if (response != null && response.Success)
            {
                return Ok(response.User);
            }
            return BadRequest(response?.Message);
        }

        /// <summary>
        /// Authenticates a user based on the provided username and password.
        /// </summary>
        /// <param name="model">The user's login credentials.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result is an <see cref="AuthenticateResponse"/> that contains the JWT token if authentication is successful.</returns>
        /// <exception cref="Exception">Thrown if an unexpected error occurs during the authentication process.</exception>
        /// <remarks>
        /// Verifies the user's credentials against the stored hash. If successful, generates a JWT token.
        /// This process is logged.
        /// </remarks>
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] UserAuthenticateModel model)
        {
            var response = await _userService.Authenticate(model);
            if (response != null && response.Success)
            {
                return Ok(response.Bearer);
            }
            return BadRequest(response?.Message);
        }

        /// <summary>
        /// Gets user profile based on username.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result is an <see cref="UserProfileResponse"/> that contains the User Profile if authentication is successful.</returns>
        /// <exception cref="Exception">Thrown if an unexpected error occurs during the retrieval process.</exception>
        /// <remarks>
        /// Tries to find the user profile based on username.
        /// This process is logged.
        /// </remarks>
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            var username = User?.Identity?.Name;
            if (username == null)
            {
                return BadRequest("invalid token");
            }

            var response = await _userService.GetUserProfile(username);

            if (response != null && response.Success)
            {
                return Ok(response.Profile);
            }


            return NotFound(response?.Message);
        }
    }
}