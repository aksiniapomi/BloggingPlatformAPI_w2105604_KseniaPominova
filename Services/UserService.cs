using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using GothamPostBlogAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GothamPostBlogAPI.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public UserService(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
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
        public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDto)
        {
            var user = await _context.Users.FindAsync(id); //search for the user by the id in the database
            if (user == null)
            {
                return false; //if doesn't exist - return false (update failed)
            }

            //Update only fields provided
            //ensure only non-null values are updated and prevent overwritting existing values with null
            if (!string.IsNullOrEmpty(userDto.Username))
                user.Username = userDto.Username;

            if (!string.IsNullOrEmpty(userDto.Email))
                user.Email = userDto.Email;

            if (!string.IsNullOrEmpty(userDto.Password))
                user.PasswordHash = _authService.HashPassword(userDto.Password); //hash new password before saving (if changed)

            _context.Users.Update(user); //mark the user as “modified” so Entity Framework updates it in the database
            await _context.SaveChangesAsync(); //save changes asynchronously to the database
            return true; //return true if sucessfull 
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