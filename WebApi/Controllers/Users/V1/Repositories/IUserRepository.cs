using WebApi.Database.Entities;

namespace WebApi.Controllers.Users.V1.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User entity);
        Task<User> FindByUsernameAsync(string username);
        Task<User> FindByEmailAsync(string email);
    }
}
