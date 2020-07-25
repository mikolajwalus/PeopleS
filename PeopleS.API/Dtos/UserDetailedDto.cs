using System;
using System.ComponentModel.DataAnnotations;

namespace PeopleS.API.Dtos
{
    public class UserDetailedDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }
        public string School { get; set; }
        public string AboutMe { get; set; }
        public string MyInterests { get; set; }
        public string PhotoUrl { get; set; }
    }
}