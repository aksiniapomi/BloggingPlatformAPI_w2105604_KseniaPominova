using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using GothamPostBlogAPI.Models.DTOs;
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
        public async Task<Like> CreateLikeAsync(LikeDTO likeDto, int userId)
        {
            //Ensure the user exists
            var user = await _context.Users.FindAsync(userId);
            var blogPost = await _context.BlogPosts.FindAsync(likeDto.BlogPostId);

            if (user == null || blogPost == null)
            {
                throw new Exception("User or BlogPost not found."); //Handle error properly
            }

            //Check if user has already liked the post
            bool alreadyLiked = await _context.Likes
                .AnyAsync(l => l.UserId == userId && l.BlogPostId == likeDto.BlogPostId);

            if (alreadyLiked)
            {
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
            return newLike;
        }

        //Remove a like (Unlike a post)
        public async Task<bool> DeleteLikeAsync(int likeId, int userId)
        {
            var like = await _context.Likes.FindAsync(likeId);
            if (like == null || like.UserId != userId) //Check and ownership
            //Ensure only the user who liked the post can remove it 
            {
                return false; //Like doesn't exist or belongs to another user
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}