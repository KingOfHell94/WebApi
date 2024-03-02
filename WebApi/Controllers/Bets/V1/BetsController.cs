using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Bets.V1.Models;
using WebApi.Controllers.Bets.V1.Services;

namespace WebApi.Controllers.Bets.V1
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BetsController : ControllerBase
    {
        private readonly IBetService _betService;

        public BetsController(IBetService betService)
        {
            _betService = betService;
        }

        /// <summary>
        /// Places bet for user.
        /// </summary>
        /// <param name="model">The user's bet model.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result is an <see cref="BetResponse"/> that contains the Bet information if betting is successful.</returns>
        /// <exception cref="Exception">Thrown if an unexpected error occurs during the retrieval process.</exception>
        /// <remarks>
        /// Tries to place bet for user if balance allows and changes the balance accordingly.
        /// This process is logged.
        /// </remarks>
        [HttpPost("place")]
        public async Task<IActionResult> PlaceBet([FromBody] PlaceBetModel model)
        {
            var username = User?.Identity?.Name;
            if (username == null) { return BadRequest(); }
            var response = await _betService.PlaceBet(username, model);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
    }
}