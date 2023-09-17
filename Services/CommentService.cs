using PenSword.Services.Interfaces;
using PenSword.Models;
using Microsoft.EntityFrameworkCore;
using PenSword.Data;

namespace PenSword.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CommentExists(int? commentId)
        {
            return (_context.Comments?.Any(e => e.Id == commentId))
                .GetValueOrDefault();
        }

        public async Task<bool> AddCommentAsync(Comment? comment)
        {
            try
            {
                if (comment == null) return false;

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Comment?> GetCommentAsync(int? commentId)
        {
            if (commentId == null) return new Comment();

            return await _context.Comments
                .Include(c => c.BlogPost)
                .FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<bool> UpdateCommentAsync(Comment? comment)
        {
            try
            {
                if (comment == null) return false;

                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCommentAsync(Comment? comment)
        {
            try
            {
                if (comment == null) return false;

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
