namespace WebApi.Controllers.Users.V1.Models
{
    public class UserProfileResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public UserProfileModel Profile { get; set; }
    }
}
