using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BookController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetAllBooks()
        {
            List<Book> books = await context.Books.ToListAsync();
            return mapper.Map<List<BookDto>>(books);
        }

        [HttpGet("{id:int}", Name = "getBookById")]
        public async Task<ActionResult<BookDtoWithAuthors>> GetBookById([FromRoute] int id)
        {
            Book book = await context.Books
                .Include(bookDB => bookDB.AuthorsBooks)
                .ThenInclude(authorBookBD => authorBookBD.Author)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound($"Book does not exist with id: {id}");
            }

            book.AuthorsBooks = book.AuthorsBooks.OrderBy(x => x.Order).ToList();

            return mapper.Map<BookDtoWithAuthors>(book);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBook([FromBody] BookCreationDto bookCreationDto)
        {
            if (bookCreationDto.AuthorsIds == null)
            {
                return BadRequest("You can't create a book without an actor.");
            }

            List<int> authorsIds = await context.Authors
                .Where(x => bookCreationDto.AuthorsIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (bookCreationDto.AuthorsIds.Count != authorsIds.Count)
            {
                return BadRequest("One of the submitted authors does not exist.");
            }

            Book book = mapper.Map<Book>(bookCreationDto);
            OrderAuthorIds(book);

            context.Add(book);
            await context.SaveChangesAsync();

            BookDto bookDto = mapper.Map<BookDto>(book);
            return CreatedAtRoute("getBookById", new { id = book.Id }, bookDto);
        }

        private void OrderAuthorIds(Book book)
        {
            if (book.AuthorsBooks != null)
            {
                for (int i = 0; i < book.AuthorsBooks.Count; i++)
                {
                    book.AuthorsBooks[i].Order = i;
                }
            }
        }
    }
}
