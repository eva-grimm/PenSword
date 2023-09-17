using PenSword.Services.Interfaces;
using PenSword.Models;
using PenSword.Data;
using Microsoft.EntityFrameworkCore;

namespace PenSword.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context) 
        {
            _context = context;
        }

        public bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task AddCategoryAsync(Category? category)
        {
            if (category == null) return;

            try
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateCategoryAsync(Category? category)
        {
            if (category == null) return;

            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteCategoryAsync(Category? category)
        {
            if (category == null) return;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<Category?> GetSingleCategoryAsync(int? categoryId)
        {
            if (categoryId == null) return new Category();
            try
            {
                Category? category = await _context.Categories
                    .Include(c => c.BlogPosts)
                    .FirstOrDefaultAsync(c => c.Id == categoryId);
                return category!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync(int? count = null)
        {
            try
            {
                IEnumerable<Category> categories = await _context.Categories
                    .Include(c => c.BlogPosts)
                    .ToListAsync();
                count ??= categories.Count();

                return categories.Take(count.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}