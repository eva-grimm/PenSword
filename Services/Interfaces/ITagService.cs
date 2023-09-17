using PenSword.Models;

namespace PenSword.Services.Interfaces
{
    public interface ITagService
    {
        public bool TagExists(int id);
        public Task<Tag?> GetSingleTagAsync(int? tagId);
        public Task<IEnumerable<Tag>> GetAllTagsAsync();
        public Task<bool> DeleteTagAsync(Tag? tag);
    }
}
