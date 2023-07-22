using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Dtos
{
    public class CommentCreateDto
    {
        [Required]
        public string Content { get; set; }
    }
}
