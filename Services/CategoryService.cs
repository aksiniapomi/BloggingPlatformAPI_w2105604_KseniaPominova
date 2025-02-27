using GothamPostBlogAPI.Data; // Import database context
using GothamPostBlogAPI.Models; // Import models
using GothamPostBlogAPI.Models.DTOs; // Import DTOs for structured data transfer
using Microsoft.EntityFrameworkCore; // Required for database queries
using Microsoft.Extensions.Logging; // Required for logging
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GothamPostBlogAPI.Services
{
    public class CategoryService // Handles category-related operations
    {
        private readonly ApplicationDbContext _context; // Query database
        private readonly ILogger<CategoryService> _logger; // Logger for tracking category actions

        public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger) // Constructor injection
        {
            _context = context;
            _logger = logger;
        }

        // Get all categories
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Fetching all categories.");
            return await _context.Categories
                .Include(c => c.BlogPosts) // Load related blog posts
                .ToListAsync();
        }

        // Get a category by ID
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            _logger.LogInformation("Fetching category with ID {CategoryId}", id);

            var category = await _context.Categories
                .Include(c => c.BlogPosts) // Load related blog posts
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found.", id);
            }

            return category;
        }

        // Create a new category using a DTO
        public async Task<Category> CreateCategoryAsync(CategoryDTO categoryDto)
        {
            _logger.LogInformation("Creating a new category: {CategoryName}", categoryDto.Name);

            var newCategory = new Category
            {
                Name = categoryDto.Name
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category {CategoryName} created successfully with ID {CategoryId}.", categoryDto.Name, newCategory.CategoryId);
            return newCategory;
        }

        // Update an existing category
        public async Task<bool> UpdateCategoryAsync(int id, CategoryDTO categoryDto)
        {
            _logger.LogInformation("Attempting to update category ID {CategoryId}.", id);

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for update.", id);
                return false;
            }

            category.Name = categoryDto.Name; // Update category name

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category with ID {CategoryId} updated successfully.", id);
            return true;
        }

        // Delete a category
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            _logger.LogInformation("Attempting to delete category ID {CategoryId}.", id);

            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found.", id);
                    return false;
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Category with ID {CategoryId} deleted successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {CategoryId}.", id);
                throw;
            }
        }
    }
}