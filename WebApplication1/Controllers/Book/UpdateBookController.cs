using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Book
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateBookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public UpdateBookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateBookAccess")]
        public IActionResult UpdateBookById(Guid id, BookModel bookModel)
        {
            {
                try
                {
                    _bookRepository.Update(bookModel);
                    return NoContent();
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

            }

        }
    }
}
