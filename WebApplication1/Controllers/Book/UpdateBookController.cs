using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Book
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateBookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public UpdateBookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateBookAccess")]
        public async Task<IActionResult> UpdateBookById(Guid id, BookUpdateModel bookModel)
        {
            try
            {
                await _bookRepository.Update(bookModel);
                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I102",
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C104",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }
    }
}
