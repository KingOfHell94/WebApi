using System.Security.Cryptography;
using System.Text;

namespace WebApi.Controllers.Users.V1.Services
{
    public class PasswordHasher
    {
        private readonly string _salt;

        public PasswordHasher(IConfiguration configuration)
        {
            _salt = configuration["PasswordHashing:Salt"];
        }

        public string HashPassword(string password)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_salt)))
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var saltedPasswordBytes = Combine(passwordBytes, Encoding.UTF8.GetBytes(_salt));
                var hashBytes = hmac.ComputeHash(saltedPasswordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }
    }
}
