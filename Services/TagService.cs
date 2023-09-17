using PenSword.Services.Interfaces;
using PenSword.Models;
using Microsoft.EntityFrameworkCore;
using PenSword.Data;

namespace PenSword.Services
{
    public class TagService : ITagService
    {
        private readonly ApplicationDbContext _context;

        public TagService (ApplicationDbContext context)
        {
            _context = context;
        }

        public bool TagExists(int id)
        {
            return (_context.Tags?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<Tag?> GetSingleTagAsync(int? tagId)
        {
            if (tagId == null) return new Tag();

            return await _context.Tags
                .Include(t => t.BlogPosts)
                .FirstOrDefaultAsync(e => e.Id == tagId);
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags
                .Include(t => t.BlogPosts)
                .ToListAsync();
        }

        public async Task<bool> DeleteTagAsync(Tag? tag)
        {
            if (tag == null) return false;

            _context.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}