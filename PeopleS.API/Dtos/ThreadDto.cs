using System;

namespace PeopleS.API.Dtos
{
    public class ThreadDto
    {
        public int UserOneId { get; set; }
        public int UserTwoId { get; set; }
        public string UserTwoPhotoUrl { get; set; }
        public string UserTwoName { get; set; }
        public string UserTwoSurname { get; set; }
        public DateTime LastModified { get; set; }
        public string Content { get; set; }
        public bool LastMessageIsMine { get; set; }
        public bool IsRead { get; set; }
    }
}