using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int BookId { get; set; }

        public Book Book { get; set; }
    }
}
