namespace PeopleS.API.Models
{
    public class ThreadParticipant
    {
        public Thread Thread { get; set; }
        public User Participant { get; set; }
        public int ThreadId { get; set; }
        public int ParticipantId { get; set; }
        public bool Status { get; set; }
    }
}