using GothamPostBlogAPI.Data; //import database context ApplicationDbContext to interact with the database 
using GothamPostBlogAPI.Models; //import models to use in the services 
using GothamPostBlogAPI.Models.DTOs;
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
        public async Task<BlogPost> CreateBlogPostAsync(BlogPostDTO blogPostDto, int userId)
        {
            //Convert DTO to Model inside the Service
            var newBlogPost = new BlogPost
            {
                Title = blogPostDto.Title,
                Content = blogPostDto.Content,
                UserId = userId, //Assign User ID from JWT
                CategoryId = blogPostDto.CategoryId
            };

            _context.BlogPosts.Add(newBlogPost);
            await _context.SaveChangesAsync();
            return newBlogPost;
        }

        //Update an existing blog post
        public async Task<bool> UpdateBlogPostAsync(int id, BlogPostDTO blogPostDto, int userId) //returns true or false
        {
            var existingBlogPost = await _context.BlogPosts.FindAsync(id); //look up the blog post by id in the database 
            if (existingBlogPost == null)
            {
                return false; //if the post doesn't exist - return false 
            }

            //Ensure only the original author or Admin can update the post
            if (existingBlogPost.UserId != userId && !UserIsAdmin(userId)) //if the logged-in user in not the original author or the user is not admin, they cannot edit the post 
            {
                return false; //deny update request 
            }

            //Convert DTO to Model inside the Service to update only specific fields 
            //enusre the userId remains unchanged (the original auhtor remains the owner)
            existingBlogPost.Title = blogPostDto.Title;
            existingBlogPost.Content = blogPostDto.Content;
            existingBlogPost.CategoryId = blogPostDto.CategoryId;

            _context.BlogPosts.Update(existingBlogPost); //mark the post as modified 
            await _context.SaveChangesAsync(); //save changes async in hte database
            return true; //true to indicate operation sucessful 
        }

        //Helper Method to Check if the User is an Admin
        private bool UserIsAdmin(int userId)
        {
            var user = _context.Users.Find(userId); //look up user by id 
            return user != null && user.Role == UserRole.Admin; //check the user exists and is an Admin 
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