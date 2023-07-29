using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using WebApplication1.Repository.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Book
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddBookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public AddBookController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateBookAccess")]
        public IActionResult CreateNew(BookModel bookModel)
        {
            try
            {
                var book = new BookModel
                {
                    id = bookModel.id,
                    Title = bookModel.Title,
                    AuthorId = bookModel.AuthorId,
                    Description = bookModel.Description,
                    PublisherId = bookModel.PublisherId,
                    PageNumber = bookModel.PageNumber,
                };
                _bookRepository.Add(book);
                return StatusCode(StatusCodes.Status201Created, book);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
