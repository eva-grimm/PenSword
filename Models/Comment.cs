using System.ComponentModel.DataAnnotations;

namespace PenSword.Models
{
    public class Comment
    {
        private DateTime _created;
        private DateTime? _updated;

        // Primary key
        public int Id { get; set; }

        // Properties
        public DateTime Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public DateTime? Updated
        {
            get => _updated;
            set => _updated = value.HasValue ? value.Value.ToUniversalTime() : null;
        }

        [Display(Name = "Update Reason")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and max {1} characters long", MinimumLength = 2)]
        public string? UpdateReason { get; set; }

        [Required, Display(Name = "Comment")]
        [StringLength(5000, ErrorMessage = "The {0} must be at least {2} and max {1} characters long", MinimumLength = 2)]
        public string? Body { get; set; }

        // Nav properties
        [Required]
        public string? AuthorId { get; set; }
        public virtual BlogUser? Author { get; set; }
        public int BlogPostId { get; set; }
        public virtual BlogPost? BlogPost { get; set; }
    }
}