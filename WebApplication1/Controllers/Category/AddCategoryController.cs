using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddCategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public AddCategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateCategoryAccess")]
        public IActionResult CreateNew(CategoryModel categoryModel)
        {
            try
            {
                var categoryExist = _categoryRepository.getByName(categoryModel.Name);

                if (categoryExist.Data == null)
                {
                    var category = new CategoryModel
                    {
                        Name = categoryModel.Name,
                        Description = categoryModel.Description,
                    };

                    Debug.WriteLine(category.ToString());

                    _categoryRepository.Add(category);

                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I201",
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C201",
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
