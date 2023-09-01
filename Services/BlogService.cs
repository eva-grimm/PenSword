﻿using Humanizer;
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

        public async Task DeleteBlogPostAsync(int? blogPostId)
        {
            BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);
            if (blogPost == null) return;

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
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
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                    .Include(b => b.Category)
                    .Include(b => b.Comments)
                        .ThenInclude(c => c.Author)
                    .Include(b => b.Tags)
                    .ToListAsync();
                return blogPosts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetBlogPostsAsync()
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                    .Where(b => b.IsPublished == true
                        && b.IsDeleted == false)
                    .Include(b => b.Category)
                    .Include(b => b.Comments)
                        .ThenInclude(c => c.Author)
                    .Include(b => b.Tags)
                    .ToListAsync();
                return blogPosts;
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
                IEnumerable<BlogPost> blogPosts = await GetBlogPostsAsync();

                return blogPosts.OrderByDescending(b => b.Comments.Count).Take(count!.Value);
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
                if (!string.IsNullOrEmpty(blogUserId))
                {
                    BlogUser? blogUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == blogUserId);
                    if (blogUser != null) return blogUser.LikedBlogPosts
                            .OrderByDescending(b => b.Created);
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

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            try
            {
                IEnumerable<Category> categories = await _context.Categories
                    .ToListAsync();
                return categories;
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
                BlogUser? blogUser = await _context.Users
                    .FirstOrDefaultAsync(b => b.Id == blogUserId);
                BlogPost? blogPost = await _context.BlogPosts
                    .FirstOrDefaultAsync(b => b.Id == blogPostId);
                if (blogUser == null || blogPost == null) return false;

                if (blogPost.UsersWhoLikeThis.Contains(blogUser)) return true;
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UserClickedLikeButtonAsync(int? blogPostId, string? blogUserId)
        {
            try
            {
                BlogUser? blogUser = await _context.Users
                    .FirstOrDefaultAsync(b => b.Id == blogUserId);
                BlogPost? blogPost = await _context.BlogPosts
                    .FirstOrDefaultAsync(b => b.Id == blogPostId);
                if (blogUser == null || blogPost == null) return;
                // if user already likes blog, then clicking button unlikes the blog
                else if (await DoesUserLikeBlogAsync(blogPostId!.Value, blogUserId!))
                {
                    blogPost.UsersWhoLikeThis.Remove(blogUser);
                    _context.Update(blogPost);
                }
                // otherwise, clicking button likes the blog
                else
                {
                    blogPost.UsersWhoLikeThis.Add(blogUser);
                    _context.Update(blogPost);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
