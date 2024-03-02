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
        private readonly IConfiguration _configuration;

        public UserService(
            IUserRepository userRepository,
            PasswordHasher passwordHasher,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<RegisterResponse> Register(UserRegisterModel model)
        {
            // Check if the username already exists
            if (await _userRepository.FindByUsernameAsync(model.Username) != null || await _userRepository.FindByEmailAsync(model.Email) != null)
            {
                return new RegisterResponse { Success = false, Message = "Username or Email already exists." };
            }

            // Hash the password
            var hashedPassword = HashPassword(model.Password);

            // Create and save the new user
            var user = new User { Username = model.Username, Email = model.Email, PasswordHash = hashedPassword, Balance = 1000 };
            await _userRepository.AddAsync(user);

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

        public async Task<AuthenticateResponse> Authenticate(UserAuthenticateModel model)
        {
            var user = await _userRepository.FindByUsernameAsync(model.Username);

            // Verify the password
            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                return new AuthenticateResponse { Success = false, Message = "Username or password is incorrect." };
            }

            // Generate JWT token
            var token = GenerateJwtToken(user.Username, _configuration);

            return new AuthenticateResponse { Success = true, Bearer = $"Bearer {token}" };
        }

        public async Task<UserProfileModel> GetUserByUsername(string username)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            if (user != null)
            {
                var result = new UserProfileModel
                {
                    Username = user.Username,
                    Email = user.Email,
                    Balance = user.Balance,
                };
                return result;
            }
            return new UserProfileModel();
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

        public static string GenerateJwtToken(string username, IConfiguration configuration)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
