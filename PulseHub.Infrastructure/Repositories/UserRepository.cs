using Microsoft.EntityFrameworkCore;
using PulseHub.Core.CustomError;
using PulseHub.Core.Models;
using PulseHub.Infrastructure.Data;

namespace PulseHub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<Result> ConfirmEmail(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return Result.Failure(new Error("NotFound", "User not found"));
                }

                user.IsActive = true;

                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error("UnexpectedError", ex.Message));
            }
        }

        public async Task<Result> DeactivateUser(string userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return Result.Failure(new Error("Not Found", "User not found"));
            }

            user.IsActive = false;
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task Delete(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserById(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Update(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException();
            }

            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
        }
    }
}
