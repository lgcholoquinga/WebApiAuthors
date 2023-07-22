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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDto>> GetBookById([FromRoute] int id)
        {
            //Book book = await context.Books.Include(bookDb => bookDb.Comments).FirstOrDefaultAsync(x => x.Id == id);

            Book book = await context.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound($"Book does not exist with id: {id}");
            }

            return mapper.Map<BookDto>(book);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBook([FromBody] BookCreationDto bookCreationDto)
        {



            //bool existAuthor = await context.Authors.AnyAsync(x => x.Id == book.AuthorId);

            //if (!existAuthor)
            //{
            //    return BadRequest($"Author does not exist with id: {book.AuthorId}");
            //}


            Book book = mapper.Map<Book>(bookCreationDto);
            context.Add(book);
            await context.SaveChangesAsync();
            return Ok("Book created.");
        }
    }
}
