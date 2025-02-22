using GothamPostBlogAPI.Data; //import database context ApplicationDbContext to interact with the database 
using GothamPostBlogAPI.Models; //import models to use in the services 
using Microsoft.EntityFrameworkCore; //used for database operations 

namespace GothamPostBlogAPI.Services
{
    public class BlogPostService //the class handles all the blog post-related logic 
    {
        private readonly ApplicationDbContext _context; //query and update database

        public BlogPostService(ApplicationDbContext context)
        {
            _context = context; //injection of the Database Context (constructor) to use the database in the service
        }

        //Get all blog posts
        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            // => Lambda operator in Lambda Expressions (mapping)
            return await _context.BlogPosts
                //For each blog post, include the User object related to it 
                .Include(blogPost => blogPost.User) //author of the blog post (User table)
                .Include(blogPost => blogPost.Category) //load related category objects 
                .Include(blogPost => blogPost.Comments)
                .Include(blogPost => blogPost.Likes)
                //Asynchronously retrieves all records from the database and converts them into a List<>
                .ToListAsync();
        }

        // Get a blog post by ID
        public async Task<BlogPost?> GetBlogPostByIdAsync(int id) //? allows for returning no matching posts (null value); nullable reference type 
        {
            return await _context.BlogPosts
                .Include(blogPost => blogPost.User)
                .Include(blogPost => blogPost.Category)
                .Include(blogPost => blogPost.Comments)
                .Include(blogPost => blogPost.Likes)
                .FirstOrDefaultAsync(blogPost => blogPost.BlogPostId == id); //.FirstOrDefaultAsync() finds the fist blog post matchin the id; returns null if not found (should be one match because id column is a primary key )
        }

        //Create a new blog post
        public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost)
        {
            blogPost.DateCreated = DateTime.UtcNow;
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        // end here // 

        //Update an existing blog post
        public async Task<bool> UpdateBlogPostAsync(int id, BlogPost updatedBlogPost)
        {
            if (id != updatedBlogPost.BlogPostId)
            {
                return false; // ID mismatch
            }

            _context.Entry(updatedBlogPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        //Delete a blog post
        public async Task<bool> DeleteBlogPostAsync(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return false;
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}