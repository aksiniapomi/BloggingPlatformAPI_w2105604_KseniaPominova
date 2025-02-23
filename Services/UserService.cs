using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GothamPostBlogAPI.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get all users
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        //Get a user by ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        //Create a new user
        public async Task<User> CreateUserAsync(User user)
        {
            //Ensure email is unique; the user has not been registered with this email before 
            var existingUser = await _context.Users.FirstOrDefaultAsync(user => user.Email == user.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Error!A user with this email already exists.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        //Update an existing user
        public async Task<bool> UpdateUserAsync(int id, User updatedUser)
        {
            if (id != updatedUser.UserId)
            {
                return false; //User ID mismatch
            }

            _context.Entry(updatedUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        //Delete a user
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}