using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class ActivityDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public virtual ICollection<AttendeeDTO> Attendees { get; set; }  
        public virtual ICollection<CommentDTO> Comments { get; set; }
    }
}
