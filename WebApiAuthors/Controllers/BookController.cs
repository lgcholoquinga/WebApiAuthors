using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public BookController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<Book> GetBookById(int id)
        {
            return await context.Books.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBook(Book book)
        {
            bool existAuthor = await context.Authors.AnyAsync(x => x.Id == book.AuthorId);

            if (!existAuthor)
            {
                return BadRequest($"Author does not exist with id: {book.AuthorId}");
            }

            context.Add(book);
            await context.SaveChangesAsync();
            return Ok("Book created.");
        }
    }
}
