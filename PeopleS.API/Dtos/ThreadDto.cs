using System;

namespace PeopleS.API.Dtos
{
    public class ThreadDto
    {
        public int UserOneId { get; set; }
        public int UserTwoId { get; set; }
        public string UserTwoPhotoUrl { get; set; }
        public DateTime LastModified { get; set; }
        public string Content { get; set; }
    }
}