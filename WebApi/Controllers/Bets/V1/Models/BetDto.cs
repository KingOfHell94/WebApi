namespace WebApi.Controllers.Bets.V1.Models
{
    public class BetDto
    {
        public decimal BetAmount { get; set; }
        public DateTime BetTime { get; set; }
        public string Details { get; set; }
    }
}
