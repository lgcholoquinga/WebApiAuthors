using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateBook([FromRoute] int id, BookCreationDto bookCreationDto)
        {
            Book bookDB = await context.Books
                .Include(auhtorsDB => auhtorsDB.AuthorsBooks)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (bookDB == null)
            {
                return NotFound($"Book does not exist with id: {id}");
            }

            Book book = mapper.Map(bookCreationDto, bookDB);
            OrderAuthorIds(book);            
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> UpdateBookPatch([FromRoute] int id, JsonPatchDocument<BookUpdateDto> bookUpdateDto)
        {
            if (bookUpdateDto == null)
            {
                return BadRequest();
            }

            Book book = await context.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound($"Book does not exist with id: {id}");
            }

            BookUpdateDto bookDto = mapper.Map<BookUpdateDto>(book);
            bookUpdateDto.ApplyTo(bookDto, ModelState);
            var isValid = TryValidateModel(bookUpdateDto);

            if (!isValid)
            {
                return BadRequest();
            }

            mapper.Map(bookDto, book);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteBook([FromRoute] int id)
        {
            bool existBook = await context.Books.AnyAsync(x => x.Id == id);

            if (!existBook) return NotFound($"Book does not exist with id:  {id}");

            context.Remove(new Book() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
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
