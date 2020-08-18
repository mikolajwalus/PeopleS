using System;
using System.Collections.Generic;

namespace PeopleS.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }
        public string School { get; set; }
        public string AboutMe { get; set; }
        public string MyInterests { get; set; }
        public string PhotoUrl { get; set; } 
        public string PublicId { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Friendship> FriendsRequested { get; set; }
        public ICollection<Friendship> FriendsRecieved { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesRecieved { get; set; }
        public ICollection<ThreadParticipant> ThreadParticipants { get; set; }
    }
}