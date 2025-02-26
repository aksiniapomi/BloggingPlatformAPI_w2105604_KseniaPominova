using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using GothamPostBlogAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GothamPostBlogAPI.Services
{
    public class CommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get all comments
        public async Task<List<Comment>> GetAllCommentsAsync() //retrieve all comments from the database with User and Blog Post 
        {
            return await _context.Comments
                .Include(comment => comment.User)       //Include User who made the comment
                .Include(comment => comment.BlogPost)   //Include BlogPost the comment is related to
                .ToListAsync();
        }

        //Get a comment by ID
        public async Task<Comment?> GetCommentByIdAsync(int id) //retrieve a single comment by ID
        {
            return await _context.Comments
                .Include(comment => comment.User)
                .Include(comment => comment.BlogPost)
                .FirstOrDefaultAsync(comment => comment.CommentId == id);
        }

        //Create a new comment in the database 
        public async Task<Comment> CreateCommentAsync(CommentDTO commentDto, int userId)
        {
            var newComment = new Comment
            {
                CommentContent = commentDto.CommentContent,
                UserId = userId, //Associate comment with the logged-in user
                BlogPostId = commentDto.BlogPostId,
                DateCreated = DateTime.UtcNow
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            return newComment;
        }

        //Update an existing comment by its ID
        public async Task<bool> UpdateCommentAsync(int id, CommentDTO commentDto, int userId)
        {
            var existingComment = await _context.Comments.FindAsync(id);
            if (existingComment == null)
            {
                return false;
            }

            //Ensure only the original author or Admin can update the comment
            if (existingComment.UserId != userId && !UserIsAdmin(userId))
            {
                return false;
            }

            existingComment.CommentContent = commentDto.CommentContent;
            _context.Comments.Update(existingComment);
            await _context.SaveChangesAsync();
            return true;
        }

        //Delete a comment by ID
        public async Task<bool> DeleteCommentAsync(int id, int userId)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return false;
            }

            //Ensure only the original author or Admin can delete the comment
            if (comment.UserId != userId && !UserIsAdmin(userId))
            {
                return false;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
        //Helper method for checking if a user is an Admin
        private bool UserIsAdmin(int userId)
        {
            var user = _context.Users.Find(userId);
            return user != null && user.Role == UserRole.Admin;
        }
    }
}
