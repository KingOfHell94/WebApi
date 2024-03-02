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

        // POST: api/bets/place
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