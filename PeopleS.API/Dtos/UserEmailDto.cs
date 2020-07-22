using System.ComponentModel.DataAnnotations;

namespace PeopleS.API.Dtos
{
    public class UserEmailDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}