namespace WebApi.Controllers.Users.V1.Models
{
    public class AuthenticateResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public string Bearer { get; set; }
    }
}
