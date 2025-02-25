using Microsoft.AspNetCore.Mvc;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
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
        private readonly UserService _userService; //business logic for users (CRUD operations)
        public UserController(ApplicationDbContext context, AuthService authService, UserService userService)
        {
            _context = context;
            _authService = authService;
            _userService = userService;
        }

        //Register a new user (default to Reader)
        [AllowAnonymous] //Anyone can register 
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            //Check if email is already registered
            if (await _context.Users.AnyAsync(user => user.Email == user.Email))
            {
                return BadRequest("Email is already registered.");
            }
            //Force the inital role to be reader to make sure new users are not allowed to decide their own role 
            user.Role = UserRole.Reader; //Only Admin can manually change a user's role 

            //Hash password before saving
            user.PasswordHash = _authService.HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        //Login a user
        [AllowAnonymous] //Anyone can log in 
        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser([FromBody] LoginRequest loginRequest)
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
            var user = await _userService.GetUserByIdAsync(id);
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
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            var userIdString = User.Identity?.Name;  //Exract user ID from JWT
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(); //Prevents parsing null values; ensure the User is authenticated 
            }
            var loggedInUserId = int.Parse(userIdString);

            if (loggedInUserId != id && !User.IsInRole(nameof(UserRole.Admin)))
            {
                return Forbid(); //Prevent updating another user’s account, unless - Admin
            }

            var success = await _userService.UpdateUserAsync(id, user);
            if (!success)
            {
                return BadRequest();
            }
            return NoContent();
        }

        //PUT - Change a user's role (Only Admins can do this) Role Management 
        [Authorize(Roles = nameof(UserRole.Admin))] //returns Admin as a compile-time constant string (retrieve the name of the enum value as a String)
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleRequest request)
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

        //DTO for updating user role (prevents unwanted data from being sent)
        public class UpdateUserRoleRequest
        {
            public UserRole NewRole { get; set; }
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

    //Login Request DTO (Data Transfer Objects) - class that defines what data the API expects from a request 
    //DTOs 1.Prevents the client from sending unwanted data
    //2.Ensure only necessary fields are passed in a request
    //3.Keep models separate from request data (easier to modify later; decoupling)
    //Instead of the full User model, only Email and Password used for login; prevents unncessary data sent in the API request - safer and more efficient 
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}