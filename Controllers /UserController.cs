using Microsoft.AspNetCore.Mvc;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Models;
using GothamPostBlogAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GothamPostBlogAPI.Data;  // This imports ApplicationDbContext
using Microsoft.Extensions.Logging; // Required for logging

namespace GothamPostBlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Private fields that store references to services and the database context; only this class can use them
        private readonly ApplicationDbContext _context; // Direct database operations 
        private readonly AuthService _authService; // Handles password hashing and JWT token generation 
        private readonly UserService _userService; // Injected via the constructor so that the controller can use methods from UserService
        private readonly ILogger<UserController> _logger; // Logger for tracking user actions

        public UserController(ApplicationDbContext context, AuthService authService, UserService userService, ILogger<UserController> logger)
        {
            _context = context;
            _authService = authService;
            _userService = userService;
            _logger = logger;
        }

        // Register a new user (allows specifying role for Admins, defaults others to Reader)
        [AllowAnonymous] // Anyone can register
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(UserDTO userDto)
        {
            // Check if email is already registered
            if (await _context.Users.AnyAsync(user => user.Email == userDto.Email))
            {
                return BadRequest("Email is already registered.");
            }

            // Assign role properly (default to Reader if not specified)
            var role = userDto.Role ?? UserRole.Reader; // Ensures role is always set

            // Explicitly set required properties
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = _authService.HashPassword(userDto.Password), // Hash password before saving
                Role = role // Assign role (Admin can specify, others default to Reader)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User {Username} registered successfully with ID {UserId}.", user.Username, user.UserId);

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        // Login a user
        [AllowAnonymous] // Anyone can log in 
        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser([FromBody] LoginRequestDTO loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginRequest.Email);

            if (user == null || !_authService.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for email: {Email}.", loginRequest.Email);
                return Unauthorized("Invalid credentials.");
            }

            var token = _authService.GenerateJwtToken(user);
            _logger.LogInformation("User {Username} logged in successfully.", user.Username);
            return Ok(new { Token = token });
        }

        // GET all users (Only Admins can view the full list of users)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _userService.GetAllUsersAsync();
        }

        // GET a single user by ID (Only the Admin and the Users themselves)
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Only the Admin or the Users themselves can access this information 
            var userIdString = User.Identity?.Name; // Extract User ID from JWT
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var loggedInUserId = int.Parse(userIdString);

            if (loggedInUserId != id && !User.IsInRole("Admin"))
            {
                return Forbid(); // Prevent unauthorized access
            }

            return user;
        }

        // PUT: Update a user profile (Users can update their own, Admins can update any)
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO userDto)
        {
            var userIdString = User.Identity?.Name;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var loggedInUserId = int.Parse(userIdString);

            if (loggedInUserId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var success = await _userService.UpdateUserAsync(id, userDto);
            if (!success)
            {
                return BadRequest();
            }

            _logger.LogInformation("User ID {UserId} updated successfully.", id);
            return NoContent();
        }

        // PUT - Change a user's role (Only Admins can do this)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDTO request)
        {
            var adminUserIdString = User.Identity?.Name;
            if (string.IsNullOrEmpty(adminUserIdString))
            {
                return Unauthorized();
            }
            var adminUserId = int.Parse(adminUserIdString);

            var success = await _userService.UpdateUserRoleAsync(id, request, adminUserId);
            if (!success)
            {
                return BadRequest("User role update failed.");
            }

            _logger.LogInformation("User ID {UserId} role updated to {NewRole} by Admin {AdminId}.", id, request.NewRole, adminUserId);
            return NoContent();
        }

        // DELETE: Remove a user (Only Admins)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound();
            }

            _logger.LogInformation("User ID {UserId} deleted successfully.", id);
            return NoContent();
        }
    }
}