using PulseHub.Core.Models;

namespace PulseHub.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserById(string id);
        Task Add(User user);
        Task Delete(string id);
        Task Update(User user);
    }
}
