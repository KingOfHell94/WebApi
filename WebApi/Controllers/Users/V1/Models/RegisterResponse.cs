namespace WebApi.Controllers.Users.V1.Models
{
    public class RegisterResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public UserDto User { get; set; }
    }
}
