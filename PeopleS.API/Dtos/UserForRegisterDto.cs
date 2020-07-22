using System.ComponentModel.DataAnnotations;

namespace PeopleS.API.Dtos
{
    public class UserForRegisterDto
    {
        [EmailAddress]
        [Required(ErrorMessage = "This field is required")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "This field is required")]
        [MaxLength(15, ErrorMessage = "Password too long")]
        [MinLength(7, ErrorMessage = "Password too short")]
        public string Password { get; set; }
    }
}