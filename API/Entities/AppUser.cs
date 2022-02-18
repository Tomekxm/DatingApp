using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime dateOfBirth { get; set; }

        public string knownAs { get; set; }
        public DateTime created { get; set; } = DateTime.Now;
        public DateTime lastActive { get; set; } = DateTime.Now;
        public string gender { get; set; }
        public string introduction { get; set; }
        public string lookingFor { get; set; }
        public string interests { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<UserLike> likedByUsers { get; set; }
        public ICollection<UserLike> likedUsers { get; set; }
        public ICollection<Message> messagesSent { get; set; }
        public ICollection<Message> messagesReceived { get; set; }
        public ICollection<AppUserRole> userRoles { get; set; }
}
    
}