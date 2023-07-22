using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public AuthorController(ApplicationDbContext context, IMapper mapper)
        {
            this._context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuthorDto>>> GetAllAuthors()
        {
            List<Author> authors = await _context.Authors.ToListAsync();
            return mapper.Map<List<AuthorDto>>(authors);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorDtoWithBooks>> GetAuthorById([FromRoute] int id)
        {
            Author author = await _context.Authors
                .Include(authorDB => authorDB.AuthorsBooks)
                .ThenInclude(authorBookDB => authorBookDB.Book)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (author == null)
            {
                return NotFound($"Author does not exist with id: {id}");
            }

            author.AuthorsBooks = author.AuthorsBooks.OrderBy(x => x.Order).ToList();

            return mapper.Map<AuthorDtoWithBooks>(author);
        }

        [HttpPost]
        public async Task<ActionResult> AddNewActor([FromBody] AuthorCreationDto authorCreationDto)
        {
            Author author = mapper.Map<Author>(authorCreationDto);
            _context.Add(author);
            await _context.SaveChangesAsync();
            return Ok("Author created.");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAuthor([FromBody] AuthorUpdateDto authorUpdateDto, [FromRoute] int id)
        {
            var existAuthor = await _context.Authors.AnyAsync(x => x.Id == id);

            if (!existAuthor)
            {
                return NotFound($"Author does not exist with id: {id}");
            }

            Author author = mapper.Map<Author>(authorUpdateDto);
            author.Id = id;

            _context.Update(author);
            await _context.SaveChangesAsync();
            return Ok("Author updated.");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAuthor([FromRoute] int id)
        {
            bool existAuthor = await _context.Authors.AnyAsync(x => x.Id == id);

            if (!existAuthor) return NotFound($"Author not exist with id:  {id}");

            _context.Remove(new Author() {Id = id});
            await _context.SaveChangesAsync();
            return Ok("Author deleted.");
        }
    }
}
