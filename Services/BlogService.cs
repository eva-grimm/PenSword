using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PenSword.Data;
using PenSword.Models;
using X.PagedList;

namespace PenSword.Services.Interfaces
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;

        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddBlogPostAsync(BlogPost? blogPost)
        {
            if (blogPost == null) return;

            try
            {
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateBlogPostAsync(BlogPost? blogPost)
        {
            if (blogPost == null) return;

            try
            {
                _context.Update(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BlogPost> GetSingleBlogPostAsync(int? blogPostId)
        {
            if (blogPostId == null) return new BlogPost();
            
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.Author)
                .Include(b => b.Tags)
                .Include(b => b.Likes)
                .FirstOrDefaultAsync(b => b.Id == blogPostId);
                return blogPost!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BlogPost> GetSingleBlogPostAsync(string? slug)
        {
            if (string.IsNullOrEmpty(slug)) return new BlogPost();
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.Author)
                .Include(b => b.Tags)
                .Include(b => b.Likes)
                .FirstOrDefaultAsync(b => b.Slug == slug);
                return blogPost!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync()
        {
            try
            {
                return await _context.BlogPosts
                    .Include(b => b.Category)
                    .Include(b => b.Comments)
                        .ThenInclude(c => c.Author)
                    .Include(b => b.Tags)
                    .Include(b => b.Likes)
                    .OrderByDescending(b => b.Created)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPublishedBlogPostsAsync()
        {
            try
            {
                return await _context.BlogPosts
                    .Where(b => b.IsPublished
                        && !b.IsDeleted)
                    .Include(b => b.Category)
                    .Include(b => b.Comments)
                        .ThenInclude(c => c.Author)
                    .Include(b => b.Tags)
                    .Include(b => b.Likes)
                    .OrderByDescending(b => b.Created)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetDraftBlogPostsAsync()
        {
            try
            {
                return await _context.BlogPosts
                    .Where(b => !b.IsPublished
                        && !b.IsDeleted)
                    .Include(b => b.Category)
                    .Include(b => b.Comments)
                        .ThenInclude(c => c.Author)
                    .Include(b => b.Tags)
                    .Include(b => b.Likes)
                    .OrderByDescending(b => b.Created)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetDeletedBlogPostsAsync()
        {
            try
            {
                return await _context.BlogPosts
                    .Where(b => !b.IsPublished
                        && b.IsDeleted)
                    .Include(b => b.Category)
                    .Include(b => b.Comments)
                        .ThenInclude(c => c.Author)
                    .Include(b => b.Tags)
                    .Include(b => b.Likes)
                    .OrderByDescending(b => b.Created)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPopularBlogPostsAsync(int? count = null)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await GetPublishedBlogPostsAsync();
                count ??= blogPosts.Count();

                return blogPosts.OrderByDescending(b => b.Comments.Count).Take(count.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetFavoriteBlogPostsAsync(string? blogUserId)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = new List<BlogPost>();

                if (string.IsNullOrEmpty(blogUserId)) return blogPosts;

                BlogUser? blogUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == blogUserId);
                if (blogUser == null) return blogPosts;

                blogPosts = await _context.BlogPosts
                    .Where(b => b.IsPublished == true
                        && b.IsDeleted == false
                        && b.Likes.Any(l => l.BlogUserId == blogUserId
                            && l.IsLiked))
                    .Include(b => b.Category)
                    .Include(b => b.Comments)
                        .ThenInclude(c => c.Author)
                    .Include(b => b.Tags)
                    .Include(b => b.Likes)
                    .OrderByDescending(b => b.Created)
                    .ToListAsync();
                return blogPosts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetBlogPostsByCategoryAsync(int? categoryId)
        {
            try
            {
                if (categoryId != null)
                {
                    Category? category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Id == categoryId);
                    if (category != null)
                    {
                        IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                            .Where(b => b.IsPublished == true
                                && b.IsDeleted == false
                                && b.CategoryId == categoryId)
                            .Include(b => b.Category)
                            .Include(b => b.Comments)
                                .ThenInclude(c => c.Author)
                            .Include(b => b.Tags)
                            .ToListAsync();
                        return blogPosts;
                    }
                }
                return Enumerable.Empty<BlogPost>();
            }
            catch (Exception)
            {
                throw;
            }
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

        public async Task DeleteCategoryAsync(int? categoryId)
        {
            Category? category = await _context.Categories.FindAsync(categoryId);
            if (category == null) return;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<Category> GetSingleCategoryAsync(int? categoryId)
        {
            if (categoryId == null) return new Category();
            try
            {
                Category? category = await _context.Categories
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

        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            try
            {
                IEnumerable<Tag> tags = await _context.Tags
                    .ToListAsync();
                return tags;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddTagsToBlogPostAsync(IEnumerable<string>? tags, int? blogPostId)
        {
            if (blogPostId == null || tags == null) return;

            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                    .FirstOrDefaultAsync(b => b.Id == blogPostId.Value);

                if (blogPost == null) return;

                foreach (string tagName in tags)
                {
                    if (string.IsNullOrEmpty(tagName.Trim())) continue;
                    Tag? tag = await _context.Tags
                        .FirstOrDefaultAsync(t => t.Name!.Trim().ToLower() == tagName.Trim().ToLower());
                    if (tag == null)
                    {
                        // if tag doesn't already exist, then we want it to; instantiate
                        tag = new() { Name = tagName.Trim().Titleize() };
                        await _context.AddAsync(tag);
                    }
                    blogPost.Tags.Add(tag);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsTagOnBlogPostAsync(int? tagId, int? blogPostId)
        {
            if (tagId == null || blogPostId == null) return false;

            try
            {
                Tag? tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);
                BlogPost? blogPost = await _context.BlogPosts
                    .Include(b => b.Tags)
                    .FirstOrDefaultAsync(b => b.Id == blogPostId);

                if (blogPost != null && tag != null) return blogPost.Tags.Contains(tag);
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveAllBlogPostTagsAsync(int? blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                    .Include(b => b.Tags)
                    .FirstOrDefaultAsync(b => b.Id == blogPostId);
                if (blogPost != null)
                {
                    blogPost.Tags.Clear();
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<BlogPost> SearchBlogPosts(string? searchString)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = new List<BlogPost>();
                if (string.IsNullOrEmpty(searchString)) return blogPosts;
                else
                {
                    searchString = searchString.Trim().ToLower();
                    blogPosts = _context.BlogPosts
                        .Where(b => b.Title!.ToLower().Contains(searchString)
                        || b.Abstract!.ToLower().Contains(searchString)
                        || b.Content!.ToLower().Contains(searchString)
                        || b.Category!.Name!.ToLower().Contains(searchString)
                        || b.Comments.Any(c => c.Body!.ToLower().Contains(searchString)
                            || c.Author!.FirstName!.ToLower().Contains(searchString)
                            || c.Author!.LastName!.ToLower().Contains(searchString))
                        || b.Tags.Any(t => t.Name!.ToLower().Contains(searchString)))
                        .Include(b => b.Category)
                        .Include(b => b.Comments)
                            .ThenInclude(c => c.Author)
                        .Include(b => b.Tags)
                        .Where(b => b.IsPublished == true && b.IsDeleted == false)
                        .AsNoTracking()
                        .OrderByDescending(b => b.Created)
                        .AsEnumerable();
                    return blogPosts;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ValidSlugAsync(string? title, int? blogPostId)
        {
            try
            {
                if (blogPostId == null || blogPostId == 0)
                {
                    return !await _context.BlogPosts.AnyAsync(b => b.Slug == title);
                }
                else
                {
                    BlogPost? blogPost = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(b => b.Id == blogPostId);
                    string? oldSlug = blogPost?.Slug;
                    if (!string.Equals(oldSlug, title))
                    {
                        return !await _context.BlogPosts.AnyAsync(b => b.Id != blogPost!.Id && b.Slug == title);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DoesUserLikeBlogAsync(int blogPostId, string blogUserId)
        {
            try
            {
                Like? like = await _context.Likes
                    .FirstOrDefaultAsync(l => l.BlogPostId == blogPostId
                        && l.BlogUserId == blogUserId
                        && l.IsLiked);
                if (like == null) return false;
                else return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UserClickedLikeButtonAsync(int blogPostId, string blogUserId)
        {
            try
            {
                Like? like = await _context.Likes
                        .FirstOrDefaultAsync(l => l.BlogPostId == blogPostId && l.BlogUserId == blogUserId);

                // if null, create Like and set IsLiked to true
                if (like == null)
                {
                    like = new Like
                    {
                        BlogPostId = blogPostId,
                        BlogUserId = blogUserId,
                        IsLiked = true,
                    };
                    _context.Add(like);
                }
                // otherwise, delete 
                else _context.Remove(like);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
