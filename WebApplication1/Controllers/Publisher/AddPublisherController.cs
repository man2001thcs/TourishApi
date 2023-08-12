using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Publisher
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddPublisherController : ControllerBase
    {
        private readonly IPublisherRepository _publisherRepository;

        public AddPublisherController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreatePublisherAccess")]
        public IActionResult CreateNew(PublisherModel publisherModel)
        {
            try
            {
                var publisherExist = _publisherRepository.getByName(publisherModel.PublisherName);

                if (publisherExist.Data == null)
                {
                    var publisher = new PublisherModel
                    {
                        PublisherName = publisherModel.PublisherName,
                        PhoneNumber = publisherModel.PhoneNumber,
                        Email = publisherModel.Email,
                        Address = publisherModel.Address,
                        Description = publisherModel.Description,
                        UpdateDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                    };

                    Debug.WriteLine(publisher.ToString());

                    _publisherRepository.Add(publisher);

                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I501",
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C501",
                    };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
