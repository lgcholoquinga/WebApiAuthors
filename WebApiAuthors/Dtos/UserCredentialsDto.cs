using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Dtos
{
    public class UserCredentialsDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
