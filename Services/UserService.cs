using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using GothamPostBlogAPI.Models.DTOs; // Import DTOs for structured data transfer
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GothamPostBlogAPI.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context; // Database context for querying/updating users
        private readonly ILogger<UserService> _logger; // Logger for tracking user actions
        private readonly AuthService _authService; // Handles authentication and password hashing

        public UserService(ApplicationDbContext context, AuthService authService, ILogger<UserService> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        // Get all users
        public async Task<List<User>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users from the database.");
            return await _context.Users.ToListAsync();
        }

        // Get a user by ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Fetching user with ID {UserId}.", id);
            return await _context.Users.FindAsync(id);
        }

        // Create a new user
        public async Task<User?> CreateUserAsync(User user)
        {
            // Ensure email is unique; the user has not been registered with this email before 
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Attempt to register with an existing email: {Email}.", user.Email);
                throw new InvalidOperationException("Error! A user with this email already exists.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User {Username} registered successfully with ID {UserId}.", user.Username, user.UserId);
            return user;
        }

        // Update an existing user
        public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDto)
        {
            var user = await _context.Users.FindAsync(id); // Search for the user by ID in the database
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update.", id);
                return false; // Return false if user does not exist
            }

            // Update only fields provided
            // Ensure only non-null values are updated and prevent overwriting existing values with null
            if (!string.IsNullOrEmpty(userDto.Username))
                user.Username = userDto.Username;

            if (!string.IsNullOrEmpty(userDto.Email))
                user.Email = userDto.Email;

            if (!string.IsNullOrEmpty(userDto.Password))
                user.PasswordHash = _authService.HashPassword(userDto.Password); // Hash new password before saving (if changed)

            _context.Users.Update(user); // Mark the user as “modified” so Entity Framework updates it in the database
            await _context.SaveChangesAsync(); // Save changes asynchronously to the database

            _logger.LogInformation("User with ID {UserId} updated successfully.", id);
            return true; // Return true if successful
        }

        // Update user role (Admin Only)
        public async Task<bool> UpdateUserRoleAsync(int id, UpdateUserRoleDTO userRoleDto, int adminUserId)
        {
            _logger.LogInformation("Admin User {AdminId} attempting to change role of User ID {UserId}.", adminUserId, id);

            // Fetch admin user details
            var adminUser = await _context.Users.FindAsync(adminUserId);
            if (adminUser == null || adminUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("Unauthorized role update attempt by User {AdminId}.", adminUserId);
                throw new UnauthorizedAccessException("Only an Admin can update user roles.");
            }

            // Find the user whose role needs to be updated
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for role update.", id);
                return false;
            }

            // Update the user's role
            user.Role = userRoleDto.NewRole;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User ID {UserId} role updated successfully to {NewRole}.", id, userRoleDto.NewRole);
            return true;
        }

        // Delete a user
        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with ID {UserId} deleted successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}.", id);
                throw;
            }
        }
    }
}