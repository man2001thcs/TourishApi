using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Author
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetAuthorController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public GetAuthorController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? search, string? sortBy, int page = 1, int pageSize = 5)
        {
            try
            {
                var authorList = _authorRepository.GetAll(search, sortBy, page, pageSize);
                return Ok(authorList);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C404",
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
                var author = _authorRepository.getById(id);
                if (author.Data == null)
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C400",
                    };
                    return NotFound(response);
                }
                else
                {
                    return Ok(author);
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C404",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}

