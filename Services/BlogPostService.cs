using GothamPostBlogAPI.Data; // Import database context ApplicationDbContext to interact with the database 
using GothamPostBlogAPI.Models; // Import models to use in the services 
using GothamPostBlogAPI.Models.DTOs; // Import DTOs for structured data transfer
using Microsoft.EntityFrameworkCore; // Used for database operations 
using Microsoft.Extensions.Logging; // Required for logging
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GothamPostBlogAPI.Services
{
    public class BlogPostService // The class handles all the blog post-related logic 
    {
        private readonly ApplicationDbContext _context; // Query and update database
        private readonly ILogger<BlogPostService> _logger; // Logger for tracking blog post actions

        public BlogPostService(ApplicationDbContext context, ILogger<BlogPostService> logger)
        {
            _context = context; // Injection of the Database Context (constructor) to use the database in the service
            _logger = logger;
        }

        // Get all blog posts
        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            _logger.LogInformation("Fetching all blog posts.");

            return await _context.BlogPosts
                .Include(blogPost => blogPost.User) // Author of the blog post (User table)
                .Include(blogPost => blogPost.Category) // Load related category objects 
                .Include(blogPost => blogPost.Comments)
                .Include(blogPost => blogPost.Likes)
                .ToListAsync(); // Asynchronously retrieves all records from the database and converts them into a List<>
        }

        // Get a blog post by ID
        public async Task<BlogPost?> GetBlogPostByIdAsync(int id)
        {
            _logger.LogInformation("Fetching blog post with ID {PostId}", id);

            var blogPost = await _context.BlogPosts
                .Include(blogPost => blogPost.User)
                .Include(blogPost => blogPost.Category)
                .Include(blogPost => blogPost.Comments)
                .Include(blogPost => blogPost.Likes)
                .FirstOrDefaultAsync(blogPost => blogPost.BlogPostId == id);

            if (blogPost == null)
            {
                _logger.LogWarning("Blog post with ID {PostId} not found.", id);
            }

            return blogPost;
        }

        // Create a new blog post
        public async Task<BlogPost> CreateBlogPostAsync(BlogPostDTO blogPostDto, int userId)
        {
            _logger.LogInformation("User {UserId} is creating a new blog post.", userId);

            var newBlogPost = new BlogPost
            {
                Title = blogPostDto.Title,
                Content = blogPostDto.Content,
                UserId = userId, // Assign User ID from JWT
                CategoryId = blogPostDto.CategoryId
            };

            _context.BlogPosts.Add(newBlogPost);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Blog post successfully created with ID {PostId} by User {UserId}.", newBlogPost.BlogPostId, userId);
            return newBlogPost;
        }

        // Update an existing blog post
        public async Task<bool> UpdateBlogPostAsync(int id, BlogPostDTO blogPostDto, int userId) // Returns true or false
        {
            _logger.LogInformation("User {UserId} is attempting to update blog post {PostId}.", userId, id);

            var existingBlogPost = await _context.BlogPosts.FindAsync(id); // Look up the blog post by id in the database 
            if (existingBlogPost == null)
            {
                _logger.LogWarning("Blog post with ID {PostId} not found for update.", id);
                return false; // If the post doesn't exist - return false 
            }

            // Ensure only the original author or Admin can update the post
            if (existingBlogPost.UserId != userId && !UserIsAdmin(userId))
            {
                _logger.LogWarning("Unauthorized update attempt: User {UserId} tried to update blog post {PostId}.", userId, id);
                return false; // Deny update request 
            }

            // Convert DTO to Model inside the Service to update only specific fields 
            // Prevent overwriting sensitive fields 
            // Ensure the userId remains unchanged (the original author remains the owner)
            existingBlogPost.Title = blogPostDto.Title;
            existingBlogPost.Content = blogPostDto.Content;
            existingBlogPost.CategoryId = blogPostDto.CategoryId;

            _context.BlogPosts.Update(existingBlogPost); // Mark the post as modified 
            await _context.SaveChangesAsync(); // Save changes async in the database

            _logger.LogInformation("Blog post with ID {PostId} successfully updated by User {UserId}.", id, userId);
            return true; // True to indicate operation successful 
        }

        // Helper Method to Check if the User is an Admin
        private bool UserIsAdmin(int userId)
        {
            var user = _context.Users.Find(userId); // Look up user by id 
            return user != null && user.Role == UserRole.Admin; // Check if the user exists and is an Admin 
        }

        // Delete a blog post
        public async Task<bool> DeleteBlogPostAsync(int blogPostId, int userId)
        {
            _logger.LogInformation("User {UserId} is attempting to delete blog post {PostId}.", userId, blogPostId);

            try
            {
                var blogPost = await _context.BlogPosts.FindAsync(blogPostId);
                if (blogPost == null)
                {
                    _logger.LogWarning("Blog post with ID {PostId} not found.", blogPostId);
                    return false;
                }

                // Ensure only the original author or Admin can delete the post
                if (blogPost.UserId != userId && !UserIsAdmin(userId))
                {
                    _logger.LogWarning("Unauthorized delete attempt: User {UserId} tried to delete blog post {PostId}.", userId);
                    return false;
                }

                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Blog post with ID {PostId} successfully deleted by User {UserId}.", blogPostId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog post with ID {PostId}.", blogPostId);
                throw;
            }
        }

        // Get blog posts by category
        public async Task<List<BlogPost>> GetBlogPostsByCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Fetching all blog posts for Category ID {CategoryId}.", categoryId);

            var blogPosts = await _context.BlogPosts
                .Where(blogPost => blogPost.CategoryId == categoryId)
                .Include(blogPost => blogPost.User)
                .Include(blogPost => blogPost.Category)
                .Include(blogPost => blogPost.Comments)
                .Include(blogPost => blogPost.Likes)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} blog posts for Category ID {CategoryId}.", blogPosts.Count, categoryId);
            return blogPosts;
        }
    }
}
