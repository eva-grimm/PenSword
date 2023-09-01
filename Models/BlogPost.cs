using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PenSword.Models
{
    public class BlogPost
    {
        private DateTime _created;
        private DateTime? _updated;

        // Primary key
        public int Id { get; set; }

        // Properties
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and max {1} characters long", MinimumLength = 2)]
        public string? Title { get; set; }

        [StringLength(600, ErrorMessage = "The {0} must be at most {1} characters long")]
        public string? Abstract { get; set; }

        [Required]
        public string? Content { get; set; }

        public DateTime Created
        {
            get => _created.ToLocalTime();
            set => _created = value.ToUniversalTime();
        }

        public DateTime? Updated
        {
            get => _updated?.ToLocalTime();
            set => _updated = value.HasValue ? value.Value.ToUniversalTime() : null;
        }

        [Required]
        public string? Slug { get; set; }

        [Display(Name = "Published?")]
        public bool IsPublished { get; set; }

        [Display(Name = "Deleted?")]
        public bool IsDeleted { get; set; }

        // Cover image
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        //Navigation Properties
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public virtual Category? Category { get; set; }
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        [JsonIgnore]
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        [JsonIgnore]
        public virtual ICollection<BlogUser> UsersWhoLikeThis { get; set; } = new HashSet<BlogUser>();
    }
}
