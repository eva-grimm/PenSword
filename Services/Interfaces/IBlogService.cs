using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PenSword.Models;

namespace PenSword.Services.Interfaces
{
    public interface IBlogService
    {
        public bool BlogPostExists(int blogPostId);
        public Task AddBlogPostAsync(BlogPost? blogPost);
        public Task UpdateBlogPostAsync(BlogPost? blogPost);
        public Task<BlogPost> GetSingleBlogPostAsync(int? blogPostId);
        public Task<BlogPost> GetSingleBlogPostAsync(string? slug);
        public Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync();
        public Task<IEnumerable<BlogPost>> GetPublishedBlogPostsAsync();
        public Task<IEnumerable<BlogPost>> GetDraftBlogPostsAsync();
        public Task<IEnumerable<BlogPost>> GetDeletedBlogPostsAsync();
        public Task AddCategoryAsync(Category? category);
        public Task UpdateCategoryAsync(Category? category);
        public Task DeleteCategoryAsync(int? categoryId);
        public Task<Category> GetSingleCategoryAsync(int? categoryId);
        public Task<IEnumerable<Category>> GetCategoriesAsync(int? count = null);
        public Task<IEnumerable<BlogPost>> GetPopularBlogPostsAsync(int? count = null);
        public Task<IEnumerable<BlogPost>> GetFavoriteBlogPostsAsync(string? blogUserId);
        public Task<IEnumerable<BlogPost>> GetBlogPostsByCategoryAsync(int? categoryId);
        public Task<IEnumerable<Tag>> GetTagsAsync();
        public Task AddTagsToBlogPostAsync(IEnumerable<string>? tags, int? blogPostId);
        public Task<bool> IsTagOnBlogPostAsync(int? tagId, int? blogPostId);
        public Task RemoveAllBlogPostTagsAsync(int? blogPostId);
        public IEnumerable<BlogPost> SearchBlogPosts(string? searchString);
        public Task<bool> ValidSlugAsync(string? title, int? blogPostId);
        public Task UserClickedLikeButtonAsync(int blogPostId, string blogUserId);
    }
}
