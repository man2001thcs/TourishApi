using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Publisher
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeletePublisherController : ControllerBase
    {
        private readonly IPublisherRepository _publisherRepository;

        public DeletePublisherController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeletePublisherAccess")]
        public IActionResult DeleteById(Guid id)
        {

            try
            {
                _publisherRepository.Delete(id);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I503",
                };
                return Ok(response);
            }
            catch
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C504",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


        }
    }
}
