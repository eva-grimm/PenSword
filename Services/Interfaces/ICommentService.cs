using PenSword.Models;

namespace PenSword.Services.Interfaces
{
    public interface ICommentService
    {
        public bool CommentExists(int? commentId);
        public Task<bool> AddCommentAsync(Comment? comment);
        public Task<Comment?> GetCommentAsync(int? commentId);
        public Task<bool> UpdateCommentAsync(Comment? comment);
        public Task<bool> DeleteCommentAsync(Comment? comment);
    }
}
