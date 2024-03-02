using Microsoft.EntityFrameworkCore;
using WebApi.Database;
using WebApi.Database.Entities;

namespace WebApi.Controllers.Users.V1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly WebApiDbContext _context;

        public UserRepository(WebApiDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User entity)
        {
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<User> FindByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<User> FindByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
