using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Author
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateAuthorController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public UpdateAuthorController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateAuthorAccess")]
        public IActionResult UpdateAuthorById(Guid id, AuthorModel AuthorModel)
        {

            try
            {
                _authorRepository.Update(AuthorModel);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I402",
                };
                return Ok(response);
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
