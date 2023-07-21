using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly MyDbContext _context;
        public BooksController(MyDbContext context)
        {
            this._context = context;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll()
        {
            var bookList = _context.Books.ToList();
            return Ok(bookList);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var book = _context.Books.SingleOrDefault(book => book.id == id);
            if (book == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(book);
            }
        }

        [HttpPost]
        public IActionResult CreateNew(BookModel bookModel)
        {
            try
            {
                var book = new Book
                {
                    id = bookModel.id,
                    AuthorId = bookModel.AuthorId,
                    Description = bookModel.Description,
                    Title = bookModel.Title,
                };
                _context.Books.Add(book);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status201Created, book);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBookById(Guid id, BookModel bookModel)
        {
            var book = _context.Books.SingleOrDefault(book => book.id == id);
            if (book == null)
            {
                return BadRequest();
            }
            else
            {
                if (bookModel.Title != null)
                {
                    book.Title = bookModel.Title;
                }

                if (bookModel.Description != null)
                {
                    book.Description = bookModel.Description;
                }

                book.AuthorId = bookModel.AuthorId;

                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteById(Guid id)
        {
            var book = _context.Books.SingleOrDefault(book => book.id == id);
            if (book == null)
            {
                return NotFound();
            }
            else
            {
                _context.Remove(book);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, book);
            }
        }
    }
}
