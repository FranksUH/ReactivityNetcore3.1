using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Comment
    {
        public string Id { get; set; }
        public string Body { get; set; }
        [ForeignKey("AppUser")]
        public string AuthorId { get; set; }
        public string ActivityId { get; set; }
        public virtual AppUser Author { get; set; }
        public virtual Activity Activity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
