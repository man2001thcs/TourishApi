using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
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

            try
            {
                _bookRepository.Delete(id);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I103",
                };
                return Ok(response);
            }
            catch
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C104",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


        }
    }
}
