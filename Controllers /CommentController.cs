using Microsoft.AspNetCore.Mvc;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Models;
using Microsoft.AspNetCore.Authorization;
using GothamPostBlogAPI.Models.DTOs;

namespace GothamPostBlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        //GET all comments (Public)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            return await _commentService.GetAllCommentsAsync();
        }

        //GET a single comment by ID (Public)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return comment;
        }

        //POST: Create a new comment (All authenticated users including Readers)
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.RegisteredUser)},{nameof(UserRole.Reader)}")]
        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment(CommentDTO commentDto)
        {
            //Extract User ID from JWT
            var userIdString = User.Identity?.Name;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdString);

            var createdComment = await _commentService.CreateCommentAsync(commentDto, userId);
            return CreatedAtAction(nameof(GetComment), new { id = createdComment.CommentId }, createdComment);
        }

        //PUT: Update a comment (Only registered Users and Admins)
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.RegisteredUser)}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, CommentDTO commentDto)
        {
            //Extract User ID from JWT
            var userIdString = User.Identity?.Name;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdString);

            var success = await _commentService.UpdateCommentAsync(id, commentDto, userId);
            if (!success)
            {
                return BadRequest();
            }
            return NoContent();
        }

        //DELETE: Remove a comment (only Admins)
        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            //Extract User ID from JWT
            var userIdString = User.Identity?.Name;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdString);

            var success = await _commentService.DeleteCommentAsync(id, userId);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}