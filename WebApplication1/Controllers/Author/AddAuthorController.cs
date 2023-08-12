using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Author
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddAuthorController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public AddAuthorController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateAuthorAccess")]
        public IActionResult CreateNew(AuthorModel authorModel)
        {
            try
            {
                var authorExist = _authorRepository.getByName(authorModel.Name);

                if (authorExist.Data == null)
                {
                    var author = new AuthorModel
                    {
                        Name = authorModel.Name,
                        PhoneNumber = authorModel.PhoneNumber,
                        Address = authorModel.Address,
                        Description = authorModel.Description,
                        UpdateDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                    };

                    Debug.WriteLine(author.ToString());

                    _authorRepository.Add(author);

                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I401",
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C401",
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
