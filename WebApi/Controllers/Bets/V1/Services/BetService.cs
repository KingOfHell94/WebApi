using Microsoft.EntityFrameworkCore;
using WebApi.Controllers.Bets.V1.Models;
using WebApi.Database;
using WebApi.Database.Entities;

namespace WebApi.Controllers.Bets.V1.Services
{


    public class BetService : IBetService
    {
        private readonly WebApiDbContext _context;

        public BetService(WebApiDbContext context)
        {
            _context = context;
        }

        public async Task<BetResponse> PlaceBet(string username, PlaceBetModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return new BetResponse { Success = false, Message = "User not found." };
            }

            if (user.Balance < model.Amount)
            {
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
    }
}
