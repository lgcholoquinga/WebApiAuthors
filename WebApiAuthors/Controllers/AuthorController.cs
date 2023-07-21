using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Author>>> GetAll()
        {
            return await _context.Authors.Include(x => x.Books).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> AddNewActor([FromBody] Author author)
        {
            _context.Add(author);
            await _context.SaveChangesAsync();
            return Ok("Author created.");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAuthor(Author author, int id)
        {
            if (author.Id != id)
            {
                return BadRequest();
            }

            _context.Update(author);
            await _context.SaveChangesAsync();
            return Ok("Author updated.");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            bool existAuthor = await _context.Authors.AnyAsync(x => x.Id == id);

            if (!existAuthor) return NotFound($"Author not exist with id:  {id}");

            _context.Remove(new Author() {Id = id});
            await _context.SaveChangesAsync();
            return Ok("Author deleted.");
        }
    }
}
