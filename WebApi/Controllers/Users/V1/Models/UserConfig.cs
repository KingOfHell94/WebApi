namespace WebApi.Controllers.Users.V1.Models
{
    public class UserConfig
    {
        public string Key { get; set; }
        public int ExpiryDurationHours { get; set; }
        public int StartingCoins { get; set; }
    }
}
