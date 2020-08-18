using Microsoft.AspNetCore.Http;

namespace PeopleS.API.Dtos
{
    public class PostForCreationDto
    {
        public int UserId { get; set; }
        public string Text { get; set; }
        public IFormFile File { get; set; }
    }
}