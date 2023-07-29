using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Book
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteBookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public DeleteBookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteBookAccess")]
        public IActionResult DeleteById(Guid id)
        {
            {
                try
                {
                    _bookRepository.Delete(id);
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
