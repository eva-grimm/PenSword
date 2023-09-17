using PenSword.Models;

namespace PenSword.Services.Interfaces
{
    public interface ICategoryService
    {
        public bool CategoryExists(int id);
        public Task AddCategoryAsync(Category? category);
        public Task UpdateCategoryAsync(Category? category);
        public Task DeleteCategoryAsync(Category? category);
        public Task<Category?> GetSingleCategoryAsync(int? categoryId);
        public Task<IEnumerable<Category>> GetCategoriesAsync(int? count = null);
    }
}
