using AutoMapper;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Utils
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AuthorCreationDto, Author>();

            CreateMap<Author, AuthorDto>();

            CreateMap<Author, AuthorDtoWithBooks>()
                .ForMember(authorDto => authorDto.Books, options => options.MapFrom(MapAuthorDtoBooks));

            CreateMap<AuthorUpdateDto, Author>();

            CreateMap<BookCreationDto, Book>()
                .ForMember(book => book.AuthorsBooks, options => options.MapFrom(MapAuthorsBooks));

            CreateMap<Book, BookDto>();

            CreateMap<Book, BookDtoWithAuthors>()
                .ForMember(bookDto => bookDto.Authors, options => options.MapFrom(MapBookDtoAuthors));

            CreateMap<BookUpdateDto, Book>().ReverseMap();

            CreateMap<CommentCreateDto, Comment>();

            CreateMap<Comment, CommentDto>();
        }

        private List<BookDto> MapAuthorDtoBooks(Author author, AuthorDto authorDto) {
        
            List<BookDto> result = new List<BookDto>();

            if (author.AuthorsBooks == null)
            {
                return result;
            }

            foreach (var authorBook in author.AuthorsBooks)
            {
                result.Add(new BookDto() { Id = authorBook.BookId, Title = authorBook.Book.Title });
            }

            return result;
        }

        private List<AuthorDto> MapBookDtoAuthors(Book book, BookDto bookDto) {
        
            List<AuthorDto> result = new List<AuthorDto>();

            if (book.AuthorsBooks == null)
            {
                return result;
            }

            foreach (var authorBook in book.AuthorsBooks)
            {
                result.Add(new AuthorDto() {  Id = authorBook.AuthorId, Name = authorBook.Author.Name });
            }

            return result;
        }

        private List<AuthorBook> MapAuthorsBooks(BookCreationDto bookCreationDto, Book book)
        {
            List<AuthorBook> result = new List<AuthorBook>();

            if (bookCreationDto.AuthorsIds == null)
            {
                return result;
            }

            foreach (var authorId in bookCreationDto.AuthorsIds)
            {
                result.Add(new AuthorBook() { AuthorId = authorId });
            }

            return result;
        }
    }
}
