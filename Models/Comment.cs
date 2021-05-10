using MVC_Blog.Enums;
using System;
using System.Collections.Generic;
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

        public string Body { get; set; }
        public DateTime Created { get; set; }

        public DateTime? Moderated { get; set; }
        public string ModeratedBody { get; set; }
        public ModerationType? ModerationType { get; set; }


        public virtual Post Post { get; set; }
        public virtual BlogUser Author { get; set; }
        public virtual BlogUser Moderator { get; set; }
    }
}
