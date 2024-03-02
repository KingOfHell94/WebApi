using WebApi.Controllers.Users.V1.Models;

namespace WebApi.Controllers.Users.V1.Services
{
    public interface IUserService
    {
        Task<RegisterResponse> Register(UserRegisterModel model);
        Task<AuthenticateResponse> Authenticate(UserAuthenticateModel model);
        Task<UserProfileModel> GetUserByUsername(string username);
    }
}
