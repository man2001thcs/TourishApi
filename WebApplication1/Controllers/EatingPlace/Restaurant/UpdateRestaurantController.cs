using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Repository.Interface.Restaurant;
using WebApplication1.Model.Restaurant;
using WebApplication1.Model.VirtualModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.EatingPlace.Restaurant
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateRestaurantController : ControllerBase
    {
        private readonly IRestaurantRepository _entityRepository;

        public UpdateRestaurantController(IRestaurantRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateRestaurantAccess")]
        public IActionResult UpdateRestaurantById(Guid id, RestaurantModel RestaurantModel)
        {

            try
            {
                var response = _entityRepository.Update(RestaurantModel);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C314",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }



        }
    }
}
