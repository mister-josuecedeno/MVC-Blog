using Microsoft.AspNetCore.Http;
using MVC_Blog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Blog.Models
{
    public class Post
    {
        /// <summary>
        /// The primary key of the post
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// This is the foreign key to the blog that is the parent to this post
        /// </summary>
        [Display(Name = "Blog Id")]
        public int BlogId { get; set; }

        /// <summary>
        /// The title of the post
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Title { get; set; }

        /// <summary>
        /// A brief introduction for reader interest
        /// </summary>
        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Abstract { get; set; }

        /// <summary>
        /// The main content of the post
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Date the post was created
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Date value when the post was updated
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }
        
        /// <summary>
        /// The SEO friendly version of the post - using the title
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Whether the post should be published
        /// </summary>
        [Required]
        [Display(Name = "Publish State")]
        public PublishState PublishState { get; set; }

        // Add Image
        /// <summary>
        /// Image Data
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// Image file type
        /// </summary>
        public string ContentType { get; set; }

        [NotMapped]
        [Display(Name = "Post Image")]
        public IFormFile ImageFile { get; set; }

        // Navigation Properties
        // Parent
        public virtual Blog Blog { get; set; }

        // Child
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }

}
