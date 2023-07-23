using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Dtos
{
    public class BookUpdateDto
    {
        [Required]
        [MinLength(3)]
        public string Title { get; set; }

        public DateTime DatePublication { get; set; }
    }
}
