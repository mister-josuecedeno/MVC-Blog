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
        public int Id { get; set; }
        public int BlogId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} and at most {1} characters.", MinimumLength = 2)]
        public string Abstract { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }
        
        public string Slug { get; set; }

        [Required]
        [Display(Name = "Publish State")]
        public PublishState PublishState { get; set; }

        // Add Image
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }

        [NotMapped]
        [Display(Name = "Post Image")]
        public IFormFile ImageFile { get; set; }

        // Navigation Properties
        public virtual Blog Blog { get; set; }
    }

}
