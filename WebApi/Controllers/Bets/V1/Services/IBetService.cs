using WebApi.Controllers.Bets.V1.Models;

namespace WebApi.Controllers.Bets.V1.Services
{
    public interface IBetService
    {
        Task<BetResponse> PlaceBet(string username, PlaceBetModel model);
    }
}
