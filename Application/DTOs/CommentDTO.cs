﻿using System;

namespace Application.DTOs
{
    public class CommentDTO
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
