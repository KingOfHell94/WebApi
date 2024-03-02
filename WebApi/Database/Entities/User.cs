namespace WebApi.Database.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }

        // Navigation property for related bets
        public virtual ICollection<Bet> Bets { get; set; }
    }
}
