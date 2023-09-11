using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PenSword.Data;
using PenSword.Models;

namespace PenSword.Services.Interfaces
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public UserService(ApplicationDbContext context,
            UserManager<BlogUser> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<BlogUser> GetUserByIdAsync(string? blogUserId)
        {
            if (string.IsNullOrEmpty(blogUserId)) return new BlogUser();

            BlogUser? blogUser = await _context.Users.FindAsync(blogUserId);

            return blogUser ?? new BlogUser();
        }

        public async Task<bool> DoesUserLikeBlogAsync(int? blogPostId, string? blogUserId)
        {
            if (string.IsNullOrEmpty(blogUserId)) return false;

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
    }
}
