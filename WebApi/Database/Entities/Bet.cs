namespace WebApi.Database.Entities
{
    public class Bet
    {
        public int BetId { get; set; }
        public int UserId { get; set; }
        public decimal BetAmount { get; set; }
        public DateTime BetTime { get; set; }
        public string Details { get; set; }

        // Navigation property for the user
        public virtual User User { get; set; }
    }
}
