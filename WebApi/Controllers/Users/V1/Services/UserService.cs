using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Database.Entities;
using WebApi.Controllers.Users.V1.Models;
using WebApi.Controllers.Users.V1.Repositories;

namespace WebApi.Controllers.Users.V1.Services
{


    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly UserConfig _userConfig;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            PasswordHasher passwordHasher,
            UserConfig jwtConfig,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _userConfig = jwtConfig;
            _logger = logger;
        }

        public async Task<RegisterResponse> Register(UserRegisterModel model)
        {
            try
            {
                _logger.LogInformation("Registering a new user with username: {Username}, email: {Email}", model.Username, model.Email);
                if (await _userRepository.FindByUsernameAsync(model.Username) != null || await _userRepository.FindByEmailAsync(model.Email) != null)
                {
                    _logger.LogError("username or email already exists username: {Username}, email: {Email}", model.Username, model.Email);
                    return new RegisterResponse { Success = false, Message = "Username or Email already exists." };
                }

                var hashedPassword = HashPassword(model.Password);

                var user = new User { Username = model.Username, Email = model.Email, PasswordHash = hashedPassword, Balance = _userConfig.StartingCoins };
                await _userRepository.AddAsync(user);

                _logger.LogInformation("Success Registering a new user with username: {Username}, email: {Email}", model.Username, model.Email);
                return new RegisterResponse
                {
                    Success = true,
                    User = new UserDto
                    {
                        Username = user.Username,
                        Email = user.Email,
                        Balance = user.Balance,
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a new user, username: {Username}, email: {Email}", model.Username, model.Email);

                return new RegisterResponse { Success = false, Message = "An error occurred during registration." };
            }

        }

        public async Task<AuthenticateResponse> Authenticate(UserAuthenticateModel model)
        {
            try
            {
                _logger.LogInformation("Authenticating user, username: {Username}", model.Username);
                var user = await _userRepository.FindByUsernameAsync(model.Username);

                if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
                {
                    _logger.LogInformation("Failed to authenticate user, username: {Username}", model.Username);
                    return new AuthenticateResponse { Success = false, Message = "Username or password is incorrect." };
                }

                var token = GenerateJwtToken(user.Username);

                _logger.LogInformation("Success Authenticating user with username: {Username}", model.Username);
                return new AuthenticateResponse { Success = true, Bearer = $"Bearer {token}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while authenticating user, username: {Username}", model.Username);

                return new AuthenticateResponse { Success = false, Message = "An error occurred during authentication" };
            }
        }

        public async Task<UserProfileResponse> GetUserProfile(string username)
        {
            try
            {
                _logger.LogInformation("Getting user profile, username: {Username}", username);
                var user = await _userRepository.FindByUsernameAsync(username);
                if (user != null)
                {
                    var result = new UserProfileModel
                    {
                        Username = user.Username,
                        Email = user.Email,
                        Balance = user.Balance,
                    };
                    _logger.LogInformation("Success Getting user profile, username: {Username}", username);
                    return new UserProfileResponse { Success = true, Profile = result };
                }
                _logger.LogInformation("Failed Getting user profile, username: {Username}", username);
                return new UserProfileResponse { Success = false, Message = "User not found" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user profile, username: {Username}", username);

                return new UserProfileResponse { Success = false, Message = "An error occurred while getting user profile" };
            }
        }

        private string HashPassword(string password)
        {
            var hashedPassword = _passwordHasher.HashPassword(password);
            return hashedPassword;
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var passwordMatches = _passwordHasher.VerifyPassword(password, hashedPassword);
            return passwordMatches;
        }

        public string GenerateJwtToken(string username)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_userConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(_userConfig.ExpiryDurationHours),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
