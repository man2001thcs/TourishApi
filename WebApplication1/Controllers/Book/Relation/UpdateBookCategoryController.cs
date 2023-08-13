﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model.VirtualModel;
using WebApplication1.Repository.Interface;
using WebApplication1.Repository.Interface.Relation;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.Book.Relation.BookStatus
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateBookCategoryController : ControllerBase
    {
        private readonly IBookCategoryRepository _bookCategoryRepository;
        private readonly char[] delimiter = new char[] { ';' };

        public UpdateBookCategoryController(IBookCategoryRepository bookCategoryRepository)
        {
            _bookCategoryRepository = bookCategoryRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateBookAccess")]
        public IActionResult UpdateBookCategoryCategory(Guid id, string RelationArrayString)
        {
            try
            {
                string[] RelationArray = RelationArrayString.Split(delimiter);

                _bookCategoryRepository.Delete(id);
                foreach (var Relation in RelationArray)
                {
                    _bookCategoryRepository.Add(id, Guid.Parse(Relation));
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
