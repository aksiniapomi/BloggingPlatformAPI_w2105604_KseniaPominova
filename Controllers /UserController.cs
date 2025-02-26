using Microsoft.AspNetCore.Mvc;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Models;
using GothamPostBlogAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GothamPostBlogAPI.Data;  //This imports ApplicationDbContext

namespace GothamPostBlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private fields that store references to services and the database context; only this class can use them
        //once assigned in the constructor, the values cannot change 
        private readonly ApplicationDbContext _context; //direct database operations 
        private readonly AuthService _authService; //handles password hashing and JWT token generation 
        private readonly UserService _userService; //injected via the constructor so that the controller can use methods from UserService

        public UserController(ApplicationDbContext context, AuthService authService, UserService userService)
        {
            _context = context;
            _authService = authService;
            _userService = userService;
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
            var role = userDto.Role ?? UserRole.Reader; //Ensures role is always set

            //Explicitly set required properties
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = _authService.HashPassword(userDto.Password), //Hash password before saving
                Role = role //Assign role (Admin can specify, others default to Reader)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        //Login a user
        [AllowAnonymous] //Anyone can log in 
        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser([FromBody] LoginRequestDTO loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginRequest.Email);

            if (user == null || !_authService.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        //GET all users (Only Admins can view the full list of users)
        [Authorize(Roles = nameof(UserRole.Admin))] //to avoid typos in admin/Admin and assign to the correct enum name 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _userService.GetAllUsersAsync();
        }

        //GET a single user by ID (Only the Admin and the Users themselves)
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id); //encapsulated logic inside UserService for better separation of concerns
            if (user == null)
            {
                return NotFound();
            }

            //Only the Admin or the Users themselves can access this information 
            var userIdString = User.Identity?.Name; //Extract User ID from JWT
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var loggedInUserId = int.Parse(userIdString);

            if (loggedInUserId != id && !User.IsInRole(nameof(UserRole.Admin)))
            {
                return Forbid(); // Prevent unauthorized access
            }
            return user;
        }

        //PUT: Update a user/ user profile details 
        //Users should be able to update their own profiles; Admins can update any profile
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO userDto)
        {
            var userIdString = User.Identity?.Name;  //Extract user ID from JWT
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(); //Prevents parsing null values; ensure the User is authenticated 
            }
            var loggedInUserId = int.Parse(userIdString);

            if (loggedInUserId != id && !User.IsInRole(nameof(UserRole.Admin)))
            {
                return Forbid(); //Prevent updating another user’s account, unless - Admin
            }

            var success = await _userService.UpdateUserAsync(id, userDto);
            if (!success)
            {
                return BadRequest();
            }
            return NoContent();
        }

        //PUT - Change a user's role (Only Admins can do this) Role Management 
        [Authorize(Roles = nameof(UserRole.Admin))] //returns Admin as a compile-time constant string (retrieve the name of the enum value as a String)
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDTO request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Role = request.NewRole;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //DELETE: Remove a user (Only Admins)
        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}