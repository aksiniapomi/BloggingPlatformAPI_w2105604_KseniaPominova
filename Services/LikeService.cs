using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GothamPostBlogAPI.Services
{
    public class LikeService
    {
        private readonly ApplicationDbContext _context;

        public LikeService(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get all likes from the database wih User and BlogPost 
        public async Task<List<Like>> GetAllLikesAsync() //async function that returns result of type T (List here)
        //while the database query is running the app remains responsive for faster performance 
        //task returns a promise of the future result 
        {
            return await _context.Likes
                .Include(like => like.User)       //Include User who liked the post
                .Include(like => like.BlogPost)   //Include the BlogPost that was liked
                .ToListAsync();
        }

        //Get a like by ID
        public async Task<Like?> GetLikeByIdAsync(int id)
        {
            return await _context.Likes
                .Include(like => like.User)
                .Include(like => like.BlogPost)
                .FirstOrDefaultAsync(like => like.LikeId == id);
        }

        //Create a new like (User likes a BlogPost)
        public async Task<Like> CreateLikeAsync(Like like)
        {
            //Check that a user cannot like the same post multiple times
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(like => like.UserId == like.UserId && like.BlogPostId == like.BlogPostId);

            if (existingLike != null)
            {
                throw new InvalidOperationException("Error!User has already liked this post.");
            }

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        //Remove a like (Unlike a post)
        public async Task<bool> DeleteLikeAsync(int id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null)
            {
                return false;
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}