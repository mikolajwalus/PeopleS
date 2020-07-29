namespace PeopleS.API.Dtos
{
    public class UserForSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; } 
    }
}