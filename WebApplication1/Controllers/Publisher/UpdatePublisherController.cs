using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Publisher
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdatePublisherController : ControllerBase
    {
        private readonly IPublisherRepository _publisherRepository;

        public UpdatePublisherController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdatePublisherAccess")]
        public IActionResult UpdatePublisherById(Guid id, PublisherModel PublisherModel)
        {

            try
            {
                _publisherRepository.Update(PublisherModel);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I502",
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C504",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }



        }
    }
}
