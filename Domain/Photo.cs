using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain
{
    public class Photo
    {
        public string Id { get; set; }
        public bool IsMain { get; set; }
        public string AppUserId { get; set; }
        [JsonIgnore]
        public virtual AppUser AppUser { get; set; }
    }
}
