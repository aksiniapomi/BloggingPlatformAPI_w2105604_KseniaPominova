using Microsoft.AspNetCore.Mvc;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Models;
using GothamPostBlogAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace GothamPostBlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //GET all categories (Public)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _categoryService.GetAllCategoriesAsync();
        }

        //GET a category by ID (Public)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

        // POST: Create a new category (Only Admins)
        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(CategoryDTO categoryDto)
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.CategoryId }, createdCategory);
        }

        //PUT: Update a category (Only Admins)
        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDTO categoryDto)
        {
            var success = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            if (!success)
            {
                return BadRequest();
            }
            return NoContent();
        }

        //DELETE: Remove a category (Only Admins)
        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}


