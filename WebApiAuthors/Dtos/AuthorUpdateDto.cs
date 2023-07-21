using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Dtos
{
    public class AuthorUpdateDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
    }
}
