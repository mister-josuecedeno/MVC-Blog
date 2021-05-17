using MVC_Blog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AuthorId { get; set; }
        public string ModeratorId { get; set; }

        [Display(Name = "User Comment")]
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public DateTime? Moderated { get; set; }  // test for null
        [Display(Name = "Moderator Comment")]
        public string ModeratedBody { get; set; }  // NullorEmpty or NullOrWhiteSpace

        [Display(Name = "Moderation Reason")]
        public ModerationType? ModerationType { get; set; } // aka ModeratedReason


        public virtual Post Post { get; set; }
        public virtual BlogUser Author { get; set; }
        public virtual BlogUser Moderator { get; set; }
    }
}
