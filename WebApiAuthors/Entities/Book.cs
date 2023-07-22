using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Title { get; set; }

        public List<Comment> Comments { get; set; }

        public List<AuthorBook> AuthorsBooks { get; set; }
    }
}
