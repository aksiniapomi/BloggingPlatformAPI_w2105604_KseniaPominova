//Controllers handle the HTTP requests (GET, POST, PUT, DELETE) call the services and return responses 
//Controllers are like traffic managers directing the HTTP requests to the right services 

using Microsoft.AspNetCore.Mvc; //allows to create a web API Controller 
using GothamPostBlogAPI.Services; //imports the service layer (BlogPostService)
using GothamPostBlogAPI.Models; //imports the data models (BlogPost)
using GothamPostBlogAPI.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GothamPostBlogAPI.Controllers
{
    [Route("api/[controller]")] //URL route api/blogposts e.g. GET /api/blogposts calles GetBlogPosts(); POST /api/blogposts calls CreateBlogPost()
    [ApiController] //enables automatic validation 
    public class BlogPostController : ControllerBase //inherits from ControllerBase with built-in API functionality 
    {
        private readonly BlogPostService _blogPostService; //dependency injection by declaring the service layer, reference to BlogPostService 

        //Inject BlogPostService constructor into the controller 
        //ASP.NET Core automatically provides BlogPostService when calling the controller 
        //For the separation of concerns and reusability of the service elsewhere 
        public BlogPostController(BlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        // GET all blog posts (Public access to all users)
        [AllowAnonymous] //no authentication required 
        [HttpGet] //route GET /api/blogposts 
        //async makes the method asynchronous - doesnt block the program while waiting for a task to complete 
        //without async the app would freeze while waiting for the database query; with async it keeps running while waiting for a response
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {
            return await _blogPostService.GetAllBlogPostsAsync(); //await pauses the method while waiting for the database response; once ready it continues the execution
                                                                  //return await - wait for the results before returning 
        }

        // GET a single blog post by ID (Public access to all users)
        [AllowAnonymous]
        [HttpGet("{id}")] // Returns the blog post defined by the ID number (GET /api/blogposts/1)
        public async Task<ActionResult<BlogPostResponseDTO>> GetBlogPost(int id)
        {
            var blogPost = await _blogPostService.GetBlogPostByIdAsync(id); // Calls GetBlogPostByIdAsync in BlogPostService
            if (blogPost == null)
            {
                return NotFound(); // Return 404 Not Found if no matching post is found
            }

            //Convert the `BlogPost` entity into `BlogPostResponseDTO` before returning
            var blogPostDto = new BlogPostResponseDTO
            {
                BlogPostId = blogPost.BlogPostId, // Assign unique post ID
                Title = blogPost.Title, // Assign title
                Content = blogPost.Content, // Assign post content
                DateCreated = blogPost.DateCreated, // Assign the date the post was created
                UserId = blogPost.UserId, // Assign the ID of the user who created the post
                Username = blogPost.User?.Username // Assign the username of the author (avoid full User object)
            };

            return Ok(blogPostDto); // Return the DTO (prevents exposing unnecessary user details)
        }

        // POST: Create a new blog post (only registered Users and Admins)
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.RegisteredUser)}")]
        [HttpPost] //route: POST /api/blogpost - endpoint of the API
        public async Task<ActionResult<BlogPost>> CreateBlogPost(BlogPostDTO blogPostDto) //use DTO instead of Model because only title,content and categoryId is needed
        {
            //Extract user ID from JWT (only the logged-in user can create a post)
            var userIdString = User.Identity?.Name;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdString);

            //Pass DTO and UserId to Service for Model conversion 
            var createdPost = await _blogPostService.CreateBlogPostAsync(blogPostDto, userId);
            return CreatedAtAction(nameof(GetBlogPost), new { id = createdPost.BlogPostId }, createdPost); //return 201 Created with new blog post 
        }

        // PUT: Update a blog post (only registered Users and Admins)
        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.RegisteredUser)}")]
        [HttpPut("{id}")] //route: PUT /api/blogposts/1
        public async Task<IActionResult> UpdateBlogPost(int id, BlogPostDTO blogPostDto) //use DTO
        {
            //Extract user ID from JWT
            var userIdString = User.Identity?.Name;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdString);

            //Pass DTO and User Id to Service 
            var success = await _blogPostService.UpdateBlogPostAsync(id, blogPostDto, userId);
            if (!success)
            {
                return BadRequest(); //returns 400 Bad Request if IDs don’t match
            }
            return NoContent();
        }

        // DELETE: Remove a blog post (Only Admins)
        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpDelete("{id}")] // Route: DELETE /api/blogposts/1
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            // Retrieve User ID from JWT token
            var userIdString = User.Identity?.Name; // Get user ID from claims
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(); // Ensure user is authenticated
            }
            var userId = int.Parse(userIdString); // Convert string to int
            // Call the BlogPostService to delete the post
            var success = await _blogPostService.DeleteBlogPostAsync(id, userId);

            if (!success)
            {
                return NotFound(); // Return 404 Not Found if post doesn't exist
            }
            return NoContent(); // Return 204 No Content (successful deletion)
        }
    }
}