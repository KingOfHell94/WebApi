using Microsoft.EntityFrameworkCore;
using WebApi.Controllers.Bets.V1.Models;
using WebApi.Controllers.Users.V1.Models;
using WebApi.Database;
using WebApi.Database.Entities;

namespace WebApi.Controllers.Bets.V1.Services
{


    public class BetService : IBetService
    {
        private readonly WebApiDbContext _context;
        private readonly ILogger<BetService> _logger;

        public BetService(
            WebApiDbContext context,
            ILogger<BetService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BetResponse> PlaceBet(string username, PlaceBetModel model)
        {
            try
            {
                _logger.LogInformation("Placing bet for user, username: {Username}", username);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    _logger.LogInformation("User not found for Placing bet, username: {Username}", username);
                    return new BetResponse { Success = false, Message = "User not found." };
                }

                if (user.Balance < model.Amount)
                {
                    _logger.LogInformation("User has insufficent balance for Placing bet, username: {Username}", username);
                    return new BetResponse { Success = false, Message = "Insufficient balance." };
                }

                var bet = new Bet
                {
                    UserId = user.UserId,
                    BetAmount = model.Amount,
                    BetTime = DateTime.UtcNow,
                    Details = model.Details,
                };

                user.Balance -= model.Amount; // Update user's balance

                await _context.Bets.AddAsync(bet);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Success Placing bet for user, username: {Username}", username);
                return new BetResponse
                {
                    Success = true,
                    Bet = new BetDto
                    {
                        BetAmount = bet.BetAmount,
                        BetTime = bet.BetTime,
                        Details = bet.Details,
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while placing bet for user, username: {Username}", username);

                return new BetResponse { Success = false, Message = "An error occurred during registration." };
            }
        }
    }
}
