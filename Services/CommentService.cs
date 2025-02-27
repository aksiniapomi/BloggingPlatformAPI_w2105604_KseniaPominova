using GothamPostBlogAPI.Data; // Import database context
using GothamPostBlogAPI.Models; // Import models
using GothamPostBlogAPI.Models.DTOs; // Import DTOs for structured data transfer
using Microsoft.EntityFrameworkCore; // Required for database queries
using Microsoft.Extensions.Logging; // Required for logging
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GothamPostBlogAPI.Services
{
    public class CommentService // Handles comment-related operations
    {
        private readonly ApplicationDbContext _context; // Query database
        private readonly ILogger<CommentService> _logger; // Logger for tracking comment actions

        public CommentService(ApplicationDbContext context, ILogger<CommentService> logger) // Constructor injection
        {
            _context = context;
            _logger = logger;
        }

        // Get all comments
        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            _logger.LogInformation("Fetching all comments.");
            return await _context.Comments
                .Include(c => c.User) // Include user who made the comment
                .Include(c => c.BlogPost) // Include the associated blog post
                .ToListAsync();
        }

        // Get a comment by ID
        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            _logger.LogInformation("Fetching comment with ID {CommentId}", id);

            var comment = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.BlogPost)
                .FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found.", id);
            }

            return comment;
        }

        // Create a new comment using a DTO
        public async Task<Comment> CreateCommentAsync(CommentDTO commentDto, int userId)
        {
            _logger.LogInformation("Adding a new comment to BlogPost ID {BlogPostId}.", commentDto.BlogPostId);

            // Ensure the blog post exists before adding a comment
            var blogPost = await _context.BlogPosts.FindAsync(commentDto.BlogPostId);
            if (blogPost == null)
            {
                _logger.LogWarning("Cannot add comment. BlogPost ID {BlogPostId} not found.", commentDto.BlogPostId);
                throw new KeyNotFoundException("Blog post not found.");
            }

            var newComment = new Comment
            {
                CommentContent = commentDto.CommentContent,
                BlogPostId = commentDto.BlogPostId,
                UserId = userId // Assign the user ID from JWT
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Comment successfully added to BlogPost ID {BlogPostId} by User ID {UserId}.", commentDto.BlogPostId, userId);
            return newComment;
        }

        // Update an existing comment
        public async Task<bool> UpdateCommentAsync(int id, CommentDTO commentDto, int userId)
        {
            _logger.LogInformation("Attempting to update comment ID {CommentId}.", id);

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found for update.", id);
                return false;
            }

            // Ensure only the original author can update the comment
            if (comment.UserId != userId)
            {
                _logger.LogWarning("User ID {UserId} is not authorized to update comment ID {CommentId}.", userId, id);
                return false;
            }

            comment.CommentContent = commentDto.CommentContent; // Update comment text

            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Comment with ID {CommentId} updated successfully by User ID {UserId}.", id, userId);
            return true;
        }

        // Delete a comment
        public async Task<bool> DeleteCommentAsync(int id, int userId)
        {
            _logger.LogInformation("Attempting to delete comment ID {CommentId}.", id);

            try
            {
                var comment = await _context.Comments.FindAsync(id);
                if (comment == null)
                {
                    _logger.LogWarning("Comment with ID {CommentId} not found.", id);
                    return false;
                }

                // Ensure only the original author or an admin can delete the comment
                if (comment.UserId != userId && !UserIsAdmin(userId))
                {
                    _logger.LogWarning("User ID {UserId} is not authorized to delete comment ID {CommentId}.", userId, id);
                    return false;
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Comment with ID {CommentId} deleted successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with ID {CommentId}.", id);
                throw;
            }
        }

        // Helper method to check if the user is an Admin
        private bool UserIsAdmin(int userId)
        {
            var user = _context.Users.Find(userId);
            return user != null && user.Role == UserRole.Admin;
        }
    }
}