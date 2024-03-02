namespace WebApi.Controllers.Bets.V1.Models
{
    public class BetResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public BetDto Bet { get; set; }
    }
}
