using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateTourCategoryController : ControllerBase
    {
        private readonly ITourishCategoryRepository _entityRepository;

        public UpdateTourCategoryController(ITourishCategoryRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateTourishCategoryAccess")]
        public IActionResult UpdateTourishCategoryById(Guid id, TourishCategoryModel TourishCategoryModel)
        {

            try
            {
                var response = _entityRepository.Update(TourishCategoryModel);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C424",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }



        }
    }
}
