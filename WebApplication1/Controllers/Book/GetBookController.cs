using Microsoft.AspNetCore.Mvc;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Book
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetBookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public GetBookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, double? from, double? to, string? sortBy, int page = 1)
        {
            try
            {
                var bookList = _bookRepository.GetAll(search, from, to, sortBy, page = 1);
                return Ok(bookList);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var book = _bookRepository.getById(id);
            if (book == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(book);
            }
        }
    }
}