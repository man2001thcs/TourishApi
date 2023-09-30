using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
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
        public IActionResult GetAll(string? search, double? from, double? to, string? sortBy, string? category, string? publisherString, string? authorString, float? saleFloat, int page = 1, int pageSize = 5)
        {
            try
            {
                var bookList = _bookRepository.GetAll(search, from, to, sortBy, category, publisherString, authorString, saleFloat, page, pageSize);
                return Ok(bookList);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C104",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var book = _bookRepository.getById(id);
                if (book.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C100",
                    };
                    return NotFound(response);
                }
                else
                {
                    return Ok(book);
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C104",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}