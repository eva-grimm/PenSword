using PenSword.Models;

namespace PenSword.Services.Interfaces
{
    public interface IUserService
    {
        public Task<BlogUser> GetUserByIdAsync(string? blogUserId);
        public Task<bool> DoesUserLikeBlogAsync(int? blogPostId, string? blogUserId);
    }
}
