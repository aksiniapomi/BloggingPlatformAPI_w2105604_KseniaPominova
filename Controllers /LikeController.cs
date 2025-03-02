using Microsoft.AspNetCore.Mvc;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Models;
using GothamPostBlogAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace GothamPostBlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly LikeService _likeService;

        public LikeController(LikeService likeService)
        {
            _likeService = likeService;
        }

        //GET all likes (Public)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Like>>> GetLikes()
        {
            return await _likeService.GetAllLikesAsync();
        }

        //GET a like by ID (Public)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Like>> GetLike(int id)
        {
            var like = await _likeService.GetLikeByIdAsync(id);
            if (like == null)
            {
                return NotFound();
            }
            return like;
        }

        //POST: Like a blog post (All authenticated users including Readers)
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.RegisteredUser)},{nameof(UserRole.Reader)}")] //If an unauthenticated User tries to access the post, 401 Unauthorized response will come up; reader will receive a 403 Forbidden Response 
        [HttpPost] // /api/likes 
        public async Task<ActionResult<Like>> CreateLike(LikeDTO likeDto) //receives a Like object in the request body 
        {
            //Extract user ID from JWT
            //Prevent errors when User.Identity.Name is null 
            //Enusre the user is authenticatd before parsing the ID
            //Return 401 Unauthorized instead of crashing
            var userIdString = User.Identity?.Name;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(); //Prevents parsing null values
            }
            //Convert user ID from string to int 
            var userId = int.Parse(userIdString); //assign the correct UserId to the Like object; ensure the user cannot like on behalf of other user 
            //Create a new Like object 
            var createdLike = await _likeService.CreateLikeAsync(likeDto, userId); //calls likeService to handle database operations; saves the Like in the database and returns saved Like object
            return CreatedAtAction(nameof(GetLikes), new { id = createdLike.LikeId }, createdLike); //incldues a refrence to the new like; nameof(GetLikes) points to the method that retrieves all likes
        }

        //DELETE: Remove a like (All authenticated users can remove their own likes)
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.RegisteredUser)},{nameof(UserRole.Reader)}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLike(int id)
        {
            //Extract User id from JWT
            var userIdString = User.Identity?.Name;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(); //Prevents parsing null values
            }
            var userId = int.Parse(userIdString); //Extract logged-in user ID from JWT 

            var success = await _likeService.DeleteLikeAsync(id, userId);
            if (!success)
            {
                return Forbid(); //Prevents deleting someone else's like
            }
            return NoContent();
        }
    }
}