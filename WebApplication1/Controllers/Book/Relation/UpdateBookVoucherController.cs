using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
using WebApplication1.Repository.Interface.Relation;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Book.Relation.BookStatus
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateBookVoucherController : ControllerBase
    {
        private readonly IBookVoucherRepository _bookVoucherRepository;
        private readonly char[] delimiter = new char[] { ';' };

        public UpdateBookVoucherController(IBookVoucherRepository bookVoucherRepository)
        {
            _bookVoucherRepository = bookVoucherRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateBookAccess")]
        public IActionResult UpdateBookVoucherVoucher(Guid id, string RelationArrayString)
        {
            try
            {
                string[] RelationArray = RelationArrayString.Split(delimiter);

                _bookVoucherRepository.Delete(id);
                foreach (var Relation in RelationArray)
                {
                    _bookVoucherRepository.Add(id, Guid.Parse(Relation));
                }

                var response = new Response
                {
                    resultCd = 0,
                    MessageCode = "I1203",
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C1204",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }
    }
}
