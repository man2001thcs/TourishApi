using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Receipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetFullReceiptController : ControllerBase
    {
        private readonly ReceiptService _receiptService;

        private readonly char[] delimiter = new char[] { ';' };

        public GetFullReceiptController(ReceiptService receiptService
            )
        {
            _receiptService = receiptService;
        }


        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_receiptService.GetFullReceiptById(id));
        }

        [HttpGet("unpaid-client")]
        [Authorize(Roles = "Admin,AdminManager")]
        public IActionResult getUnpaidClient()
        {
            return Ok(_receiptService.getUnpaidClient());
        }

        [HttpGet("gross-tour")]
        [Authorize(Roles = "Admin,AdminManager")]
        public IActionResult getTopGrossTourInMonth()
        {
            return Ok(_receiptService.getTopGrossTourInMonth());
        }

        [HttpGet("total-ticket-tour")]
        [Authorize(Roles = "Admin,AdminManager")]
        public IActionResult getTopTicketTourInMonth()
        {
            return Ok(_receiptService.getTopTicketTourInMonth());
        }

        [HttpGet("gross-moving-service")]
        [Authorize(Roles = "Admin,AdminManager")]
        public IActionResult getTopGrossMovingScheduleInMonth()
        {
            return Ok(_receiptService.getTopGrossMovingScheduleInMonth());
        }

        [HttpGet("gross-staying-service")]
        [Authorize(Roles = "Admin,AdminManager")]
        public IActionResult getTopGrossStayingScheduleInMonth()
        {
            return Ok(_receiptService.getTopGrossStayingScheduleInMonth());
        }

        [HttpGet("total-ticket-moving-service")]
        [Authorize(Roles = "Admin,AdminManager")]
        public IActionResult getTopTicketMovingScheduleInMonth()
        {
            return Ok(_receiptService.getTopTicketMovingScheduleInMonth());
        }

        [HttpGet("total-ticket-staying-service")]
        [Authorize(Roles = "Admin,AdminManager")]
        public IActionResult getTopTicketStayingScheduleInMonth()
        {
            return Ok(_receiptService.getTopTicketStayingScheduleInMonth());
        }

        [HttpGet("tourish-plan/total-month-gross")]
        [Authorize(Roles = "Admin,AdminManager")]
        public IActionResult getGrossTourishPlanInYear()
        {
            return Ok(_receiptService.getGrossTourishPlanInYear());
        }

        [HttpGet("moving-schedule/total-month-gross")]
        [Authorize(Roles = "Admin,AdminManager")]
        public IActionResult getGrossMovingScheduleInYear()
        {
            return Ok(_receiptService.getGrossMovingScheduleInYear());
        }

        [HttpGet("staying-schedule/total-month-gross")]
        [Authorize(Roles = "Admin,AdminManager")]
        public IActionResult getGrossStayingScheduleInYear()
        {
            return Ok(_receiptService.getGrossStayingScheduleInYear());
        }
    }
}