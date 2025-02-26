using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using GothamPostBlogAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

//Only Admin can create, update, or delete categories

namespace GothamPostBlogAPI.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context; //parameter passed to contructor from Program.cs
            //_context is a private field that stores context 
            //assings the context from constructor to the field; _ avoids naming conflicts
        }

        //Get all categories
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Get a category by ID
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        //Create a new category
        public async Task<Category> CreateCategoryAsync(CategoryDTO categoryDto)
        {
            var newCategory = new Category
            {
                Name = categoryDto.Name
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return newCategory;
        }

        //Update an existing category
        public async Task<bool> UpdateCategoryAsync(int id, CategoryDTO categoryDto)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return false;
            }

            existingCategory.Name = categoryDto.Name;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();
            return true;
        }

        //Delete a category
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