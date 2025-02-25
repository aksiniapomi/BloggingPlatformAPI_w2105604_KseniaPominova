using Microsoft.AspNetCore.Mvc;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace GothamPostBlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        //GET all users (Only Admins can view the full list of users)
        [Authorize(Roles = "Admin")]
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
            var loggedInUserId = int.Parse(User.Identity.Name); //Extract User ID from JWT
            if (user.UserId != loggedInUserId && !User.IsInRole("Admin"))
            {
                return Forbid(); //Access denied
            }

            return user;
        }

        //POST: Create a new user (anyone can register - new users)
        [AllowAnonymous] //anyone should be able to register without logging in 
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
        }

        //PUT: Update a user
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

            if (loggedInUserId != id && !User.IsInRole("Admin"))
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

        //DELETE: Remove a user (Only Admins)
        [Authorize(Roles = "Admin")]
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