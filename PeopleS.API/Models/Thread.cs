using System;
using System.Collections.Generic;

namespace PeopleS.API.Models
{
    public class Thread
    {
        public int Id { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<ThreadParticipant> ThreadParticipants { get; set; }
        public DateTime LastModified { get; set; }
    }
}