using AutoMapper;
using LibraryApi.Domain;
using LibraryApi.Filters;
using LibraryApi.Models.Books;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class BooksController : ControllerBase
    {
        private readonly IBookCommands _bookCommands;
        private readonly ILookupBooks _bookLookup;

        public BooksController(LibraryDataContext context, IMapper mapper, ILookupBooks bookLookup, IBookCommands bookCommands = null)
        {
            _bookLookup = bookLookup;
            _bookCommands = bookCommands;
        }

        // PUT /books/{id}/genre
        [HttpPut("books/{id:int}/genre")]
        public async Task<ActionResult> UpdateGenre(int id, [FromBody] string genre)
        {
            var isSuccess = await _bookCommands.UpdateBookGenre(id, genre);

            return isSuccess ? NoContent() : (ActionResult)NotFound();
        }

        [HttpDelete("books/{id:int}")]
        public async Task<ActionResult> RemoveBookFromInventory(int id)
        {
            await _bookCommands.RemoveBookFromInventory(id);

            return NoContent();
        }

        [ValidateModel]
        [HttpPost("books")]
        public async Task<ActionResult> AddBook([FromBody] PostBookRequest request)
        {
            var response = await _bookCommands.AddBookToInventory(request);

            return CreatedAtRoute("books#getbookbyid", new { id = response.Id }, response);
        }

        [HttpGet("books/{id:int}", Name = "books#getbookbyid")]
        public async Task<ActionResult> GetBookById(int id)
        {
            GetBookDetailsResponse response = await _bookLookup.GetBookById(id);

            return this.Maybe(response);
        }

        [HttpGet("books")]
        public async Task<ActionResult> GetAllBooks([FromQuery] string genre = "All")
        {
            GetBooksResponse response = await _bookLookup.GetBooks(genre);
            return Ok(response);
        }
    }
}