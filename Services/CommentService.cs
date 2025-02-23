using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
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
        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments
                .Include(comment => comment.User)       //Include User who made the comment
                .Include(comment => comment.BlogPost)   //Include BlogPost the comment is related to
                .ToListAsync();
        }

        //Get a comment by ID
        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(comment => comment.User)
                .Include(comment => comment.BlogPost)
                .FirstOrDefaultAsync(comment => comment.CommentId == id);
        }

        //Create a new comment
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            comment.DateCreated = DateTime.UtcNow;  //Set timestamp
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        //Update an existing comment
        public async Task<bool> UpdateCommentAsync(int id, Comment updatedComment)
        {
            if (id != updatedComment.CommentId)
            {
                return false; //ID mismatch
            }

            _context.Entry(updatedComment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        //Delete a comment
        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return false;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
