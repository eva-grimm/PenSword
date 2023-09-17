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
        public Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync(string? authorId = null);
        public Task<IEnumerable<BlogPost>> GetPublishedBlogPostsAsync(string? authorId = null);
        public Task<IEnumerable<BlogPost>> GetDraftBlogPostsAsync(string? authorId = null);
        public Task<IEnumerable<BlogPost>> GetDeletedBlogPostsAsync(string? authorId = null);
        public Task<IEnumerable<BlogPost>> GetPopularBlogPostsAsync(int? count = null);
        public Task<IEnumerable<BlogPost>> GetLikedBlogPostsAsync(string? blogUserId);
        public Task<IEnumerable<BlogPost>> GetBlogPostsByCategoryAsync(int? categoryId);
        public Task<IEnumerable<BlogPost>> GetBlogPostsByTagAsync(int? categoryId);
        public Task<IEnumerable<Tag>> GetTagsAsync();
        public Task AddTagsToBlogPostAsync(IEnumerable<string>? tags, int? blogPostId);
        public Task<bool> IsTagOnBlogPostAsync(int? tagId, int? blogPostId);
        public Task RemoveAllBlogPostTagsAsync(int? blogPostId);
        public IEnumerable<BlogPost> SearchBlogPosts(string? searchString);
        public Task<bool> ValidSlugAsync(string? title, int? blogPostId);
        public Task UserClickedLikeButtonAsync(int blogPostId, string blogUserId);
    }
}
