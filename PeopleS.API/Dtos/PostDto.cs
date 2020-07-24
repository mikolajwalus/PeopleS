using System;

namespace PeopleS.API.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Photo { get; set; }
        public DateTime DateOfCreation { get; set; }
        public int UserId { get; set; }
        public UserForPostDto User { get; set; }

    }
}