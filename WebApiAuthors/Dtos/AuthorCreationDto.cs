using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Dtos
{
    public class AuthorCreationDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
    }
}
