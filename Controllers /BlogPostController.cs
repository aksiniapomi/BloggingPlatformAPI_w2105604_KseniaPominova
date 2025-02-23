//Controllers handle the HTTP requests (GET, POST, PUT, DELETE) call the services and return responses 
//Controllers are like traffic managers directing the HTTP requests to the right services 

using Microsoft.AspNetCore.Mvc; //allows to create a web API Controller 
using GothamPostBlogAPI.Services; //imports the service layer (BlogPostService)
using GothamPostBlogAPI.Models; //imports the data models (BlogPost)
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
        [HttpGet("{id}")] //returns the blog posts defined by the id number GET /api/blogposts/1 
        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            var blogPost = await _blogPostService.GetBlogPostByIdAsync(id); //calls the GetBlogPostByIdAsync in BlogPostService
            if (blogPost == null)
            {
                return NotFound(); //return 404 Not Found if no matching posts found 
            }
            return blogPost;
        }

        // POST: Create a new blog post (only registered Users and Admins)
        [Authorize(Roles = "Admin, RegisteredUser")]
        [HttpPost] //route: POST /api/blogposts 
        public async Task<ActionResult<BlogPost>> CreateBlogPost(BlogPost blogPost)
        {
            var createdPost = await _blogPostService.CreateBlogPostAsync(blogPost); //calls CreateBlogPostAsync(blogPost) in BlogPostService
            return CreatedAtAction(nameof(GetBlogPost), new { id = createdPost.BlogPostId }, createdPost); //return 201 Created with new blog post 
        }

        // PUT: Update a blog post (only registered Users and Admins)
        [Authorize(Roles = "Admin, RegisteredUser")]
        [HttpPut("{id}")] //route: PUT /api/blogposts/1
        public async Task<IActionResult> UpdateBlogPost(int id, BlogPost blogPost)
        {
            var success = await _blogPostService.UpdateBlogPostAsync(id, blogPost); //calls UpdateBlogPostAsync(id, blogPost) in BlogPostService
            if (!success)
            {
                return BadRequest(); //returns 400 Bad Request if IDs don’t match
            }
            return NoContent();
        }

        // DELETE: Remove a blog post (Only Admins)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")] //route: DELETE /api/blogposts/1 
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var success = await _blogPostService.DeleteBlogPostAsync(id); //calls DeleteBlogPostAsync(id) in BlogPostService
            if (!success)
            {
                return NotFound(); //return 404 Not Found if post doesn't exist 
            }
            return NoContent();
        }
    }
}