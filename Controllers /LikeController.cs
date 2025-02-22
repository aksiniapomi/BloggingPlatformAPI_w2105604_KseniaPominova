using Microsoft.AspNetCore.Mvc;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Models;

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

        //GET all likes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Like>>> GetLikes()
        {
            return await _likeService.GetAllLikesAsync();
        }

        //GET a like by ID
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

        //POST: Like a blog post
        [HttpPost]
        public async Task<ActionResult<Like>> CreateLike(Like like)
        {
            var createdLike = await _likeService.CreateLikeAsync(like);
            return CreatedAtAction(nameof(GetLike), new { id = createdLike.LikeId }, createdLike);
        }

        //DELETE: Remove a like
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLike(int id)
        {
            var success = await _likeService.DeleteLikeAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}