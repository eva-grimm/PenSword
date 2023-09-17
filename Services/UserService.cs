using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PenSword.Data;
using PenSword.Models;
using PenSword.Services.Interfaces;

namespace PenSword.Services
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

        public bool UserExists(string userId)
        {
            return (_context.Users?.Any(e => e.Id == userId)).GetValueOrDefault();
        }

        public async Task UpdateUser(BlogUser? user)
        {
            if (user == null) return;

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BlogUser> GetUserByIdAsync(string? blogUserId)
        {
            if (string.IsNullOrEmpty(blogUserId)) return new BlogUser();

            BlogUser? blogUser = await _context.Users.FindAsync(blogUserId);

            return blogUser ?? new BlogUser();
        }

        public async Task<IEnumerable<BlogUser>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Authored)
                .Include(u => u.Comments)
                .ToListAsync();
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