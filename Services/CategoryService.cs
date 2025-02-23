using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GothamPostBlogAPI.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Get all categories
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // ✅ Get a category by ID
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        // ✅ Create a new category
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        // ✅ Update an existing category
        public async Task<bool> UpdateCategoryAsync(int id, Category updatedCategory)
        {
            if (id != updatedCategory.CategoryId)
            {
                return false; // ID mismatch
            }

            _context.Entry(updatedCategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Delete a category
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}