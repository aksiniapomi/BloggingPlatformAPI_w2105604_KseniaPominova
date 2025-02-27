using GothamPostBlogAPI.Data; // Import database context ApplicationDbContext to interact with the database
using GothamPostBlogAPI.Models; // Import models to use in the service
using GothamPostBlogAPI.Models.DTOs; // Import DTOs for structured data transfer
using Microsoft.EntityFrameworkCore; // Used for database operations
using Microsoft.Extensions.Logging; // Required for logging
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GothamPostBlogAPI.Services
{
    public class LikeService
    {
        private readonly ApplicationDbContext _context; // Allows querying and updating the database
        private readonly ILogger<LikeService> _logger; // Logger for tracking like-related actions

        public LikeService(ApplicationDbContext context, ILogger<LikeService> logger) //Injecting Logger
        {
            _context = context;
            _logger = logger;
        }

        // Get all likes from the database with User and BlogPost 
        public async Task<List<Like>> GetAllLikesAsync() // Async function that returns result of type T (List here)
        {
            _logger.LogInformation("Fetching all likes from the database.");

            return await _context.Likes
                .Include(like => like.User)       // Include User who liked the post
                .Include(like => like.BlogPost)   // Include the BlogPost that was liked
                .ToListAsync();
        }

        // Get a like by ID
        public async Task<Like?> GetLikeByIdAsync(int id)
        {
            _logger.LogInformation("Fetching like with ID {LikeId}", id);

            return await _context.Likes
                .Include(like => like.User)
                .Include(like => like.BlogPost)
                .FirstOrDefaultAsync(like => like.LikeId == id);
        }

        // Create a new like (User likes a BlogPost)
        public async Task<Like> CreateLikeAsync(LikeDTO likeDto, int userId)
        {
            _logger.LogInformation("User {UserId} is attempting to like BlogPost {BlogPostId}", userId, likeDto.BlogPostId); // ✅ Added logging

            // Ensure the user exists
            var user = await _context.Users.FindAsync(userId);
            var blogPost = await _context.BlogPosts.FindAsync(likeDto.BlogPostId);

            if (user == null || blogPost == null)
            {
                _logger.LogWarning("User {UserId} or BlogPost {BlogPostId} not found.", userId, likeDto.BlogPostId);
                throw new Exception("User or BlogPost not found."); // Handle error properly
            }

            // Check if user has already liked the post
            bool alreadyLiked = await _context.Likes
                .AnyAsync(l => l.UserId == userId && l.BlogPostId == likeDto.BlogPostId);

            if (alreadyLiked)
            {
                _logger.LogWarning("User {UserId} has already liked BlogPost {BlogPostId}.", userId, likeDto.BlogPostId);
                throw new Exception("User has already liked this post.");
            }

            var newLike = new Like
            {
                UserId = userId,
                BlogPostId = likeDto.BlogPostId,
                User = user,
                BlogPost = blogPost
            };

            _context.Likes.Add(newLike);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} successfully liked BlogPost {BlogPostId}.", userId, likeDto.BlogPostId);
            return newLike;
        }

        // Remove a like (Unlike a post)
        public async Task<bool> DeleteLikeAsync(int likeId, int userId)
        {
            _logger.LogInformation("User {UserId} is attempting to remove like {LikeId}.", userId, likeId);

            var like = await _context.Likes.FindAsync(likeId);
            if (like == null)
            {
                _logger.LogWarning("Like {LikeId} not found.", likeId);
                return false;
            }

            // Ensure only the user who liked the post can remove it 
            if (like.UserId != userId)
            {
                _logger.LogWarning("Unauthorized attempt: User {UserId} tried to remove like {LikeId} belonging to another user.", userId, likeId);
                return false;
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} successfully removed like {LikeId}.", userId, likeId);
            return true;
        }
    }
}