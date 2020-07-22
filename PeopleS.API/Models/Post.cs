using System;

namespace PeopleS.API.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Photo { get; set; }
        public DateTime DateOfCreation { get; set; }
        public int UserId { get; set; }
    }
}