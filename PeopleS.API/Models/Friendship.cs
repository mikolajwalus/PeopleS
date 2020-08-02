namespace PeopleS.API.Models
{
    public class Friendship
    {
        public User Requestor { get; set; }
        public User Reciever { get; set; }
        public int RequestorId { get; set; }
        public int RecieverId { get; set; }
        /// 0 - invited
        /// 1 - accepted
        /// 2 - blocked
        /// 3 - none
        /// 4 - yourself (not exist in database)
        /// 5 - invitation sent (not appearing in database but as result)
        public int Status { get; set; } = 0;
    }
}