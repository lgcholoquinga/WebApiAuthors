namespace WebApiAuthors.Entities
{
    public class AuthorBook
    {
        public int BookId { get; set; }

        public int AuhtorId { get; set; }

        public int Order { get; set; }

        public Book Book { get; set; }

        public Author Author { get; set; }
    }
}
