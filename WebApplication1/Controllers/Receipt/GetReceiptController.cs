﻿using Microsoft.AspNetCore.Mvc;
using TourishApi.Service.InheritanceService;
using WebApplication1.Data.Receipt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Receipt
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetReceiptController : ControllerBase
    {
        private readonly ReceiptService _receiptService;

        private readonly char[] delimiter = new char[] { ';' };

        public GetReceiptController(ReceiptService receiptService
            )
        {
            _receiptService = receiptService;
        }


        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult GetAll(string? tourishPlanId, string? sortBy, int page = 1, int pageSize = 5, ReceiptStatus status = ReceiptStatus.Created)
        {
            return Ok(_receiptService.GetAll(tourishPlanId, sortBy, page, pageSize, status));

        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_receiptService.GetById(id));
        }
    }
}