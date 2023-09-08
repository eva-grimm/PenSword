namespace PenSword.Models
{
    public class Like
    {
        // Primary Key
        public int Id { get; set; }

        // Properties
        public int BlogPostId { get; set; }
        public string? BlogUserId { get; set; }
        public bool IsLiked { get; set; }

        // Nav properties
        public BlogPost? BlogPost { get; set; }
        public BlogUser? BlogUser { get; set; }
    }
}
