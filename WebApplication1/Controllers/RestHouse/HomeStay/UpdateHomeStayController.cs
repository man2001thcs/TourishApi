﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourishApi.Repository.Interface.Resthouse;
using WebApplication1.Model.RestHouse;
using WebApplication1.Model.VirtualModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers.RestHouse.HomeStay
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateHomeStayController : ControllerBase
    {
        private readonly IHomeStayRepository _entityRepository;

        public UpdateHomeStayController(IHomeStayRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdateHomeStayAccess")]
        public IActionResult UpdateHomeStayById(Guid id, HomeStayModel HomeStayModel)
        {

            try
            {
                var response = _entityRepository.Update(HomeStayModel);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    resultCd = 1,
                    MessageCode = "C224",
                    Error = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }



        }
    }
}
