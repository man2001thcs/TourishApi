using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.TourCategory
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddTourCategoryController : ControllerBase
    {
        private readonly ITourishCategoryRepository _entityRepository;

        public AddTourCategoryController(ITourishCategoryRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateTourishCategoryAccess")]
        public IActionResult CreateNew(TourishCategoryModel entityModel)
        {
            try
            {
                var response = _entityRepository.Add(entityModel);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
