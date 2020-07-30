namespace PeopleS.API.Models
{
    public class Friendship
    {
        public User Requestor { get; set; }
        public User Reciever { get; set; }
        public int RequestorId { get; set; }
        public int RecieverId { get; set; }
        public int Status { get; set; } = 0;
    }
}