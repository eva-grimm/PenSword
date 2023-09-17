using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PenSword.Models
{
    public class BlogUser : IdentityUser
    {
        [Required, Display(Name  = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long", MinimumLength = 2)]
        public string? FirstName { get; set; }
        
        [Required, Display(Name  = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long", MinimumLength = 2)]
        public string? LastName { get; set; }

        [NotMapped]
        public string? FullName { get { return $"{FirstName} {LastName}"; } }

        // User Profile Image
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        // Author Details
        public string? JobTitle { get; set; }
        public string? Company { get; set; }
        public string? Byline { get; set; }
        public string? Bio { get; set; }

        // Socials
        public string? LinkedIn { get; set; }
        public string? GitHub { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Website { get; set; }

        // Navigation Properties
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new HashSet<Like>();
        public virtual ICollection<BlogPost> Authored { get; set; } = new HashSet<BlogPost>();
    }
}