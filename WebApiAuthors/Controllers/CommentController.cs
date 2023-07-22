﻿using AutoMapper;
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
            return Ok("Comment created.");
        }
      
    }
}
