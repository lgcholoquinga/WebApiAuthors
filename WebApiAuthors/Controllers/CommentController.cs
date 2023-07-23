using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers
{
    [Route("api/Book/{bookId:int}/Comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CommentController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "getCommentById")]
        public async Task<ActionResult<CommentDto>> GetCommentById([FromRoute] int id)
        {
            Comment comment = await context.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if(comment == null)
            {
                return NotFound($"Coment does not exist with id: {id}");
            }

            CommentDto commentDto = mapper.Map<CommentDto>(comment);
            return Ok(commentDto);
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDto>>> getCommentsByBookId(int bookId)
        {
            List<Comment> comments = await context.Comments.Where(x => x.BookId == bookId).ToListAsync();
            return mapper.Map<List<CommentDto>>(comments);
        }

        [HttpPost]
        public async Task<ActionResult> CreateComment([FromRoute] int bookId, CommentCreateDto commentCreateDto)
        {
            bool existBook = await context.Books.AnyAsync(x => x.Id == bookId);

            if (!existBook)
            {
                return NotFound($"Book does not exist with id {bookId}");
            }

            Comment comment = mapper.Map<Comment>(commentCreateDto);
            comment.BookId = bookId;
            context.Add(comment);
            await context.SaveChangesAsync();

            CommentDto commentDto = mapper.Map<CommentDto>(comment);

            return CreatedAtRoute("getCommentById", new { id = comment.Id, bookId}, commentDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateComment([FromRoute] int bookId,[FromRoute] int id, CommentCreateDto commentCreateDto)
        {
            bool existBook = await context.Books.AnyAsync(x => x.Id == bookId);

            if (!existBook)
            {
                return NotFound($"Book does not exist with id: {bookId}");
            }

            bool existComment = await context.Comments.AnyAsync(x => x.Id == id);

            if (!existComment)
            {
                return NotFound($"Comment does not exist with id: {id}");
            }

            Comment comment = mapper.Map<Comment>(commentCreateDto);
            comment.Id = id;
            comment.BookId = bookId;
            context.Update(comment);
            await context.SaveChangesAsync();
            return NoContent();
        }
      
    }
}
