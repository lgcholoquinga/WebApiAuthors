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
            CreateMap<AuthorUpdateDto, Author>();
            CreateMap<BookCreationDto, Book>();
            CreateMap<Book, BookDto>();
        }
    }
}
