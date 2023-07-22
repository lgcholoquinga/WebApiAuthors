using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Entities
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        public List<AuthorBook> AuthorsBooks { get; set; }
    }
}
