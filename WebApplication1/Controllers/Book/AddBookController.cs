using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Model;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Book
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddBookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookStatusRepository _bookStatusRepository;

        private readonly char[] delimiter = new char[] { ';' };

        public AddBookController(IBookRepository bookRepository, IBookStatusRepository bookStatusRepository
            )
        {
            _bookRepository = bookRepository;
            _bookStatusRepository = bookStatusRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateBookAccess")]
        public async Task<IActionResult> CreateNew(BookInsertModel bookInsertModel)
        {
            try
            {

                var bookExist = _bookRepository.getByName(bookInsertModel.Title);

                if (bookExist.Data == null)
                {
                    var bookReturn = await _bookRepository.Add(bookInsertModel);

                    var bookReturnId = (Guid)bookReturn.returnId;

                    Debug.Write(bookReturnId);

                    var response = new Response
                    {
                        resultCd = 0,
                        MessageCode = "I101",
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C104",
                    };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C105",
                    Error = ex.Message,
                    Data = ex

                };
                return BadRequest(response);
            }
        }
    }
}
