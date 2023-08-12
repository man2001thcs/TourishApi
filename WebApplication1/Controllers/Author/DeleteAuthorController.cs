using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Author
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteAuthorController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public DeleteAuthorController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteAuthorAccess")]
        public IActionResult DeleteById(Guid id)
        {
            {
                try
                {
                    _authorRepository.Delete(id);
                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I403",
                    };
                    return Ok(response);
                }
                catch
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C404",
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

            }
        }
    }
}
