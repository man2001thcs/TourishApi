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
    public class AddHomeStayController : ControllerBase
    {
        private readonly IHomeStayRepository _entityRepository;

        public AddHomeStayController(IHomeStayRepository airPlaneRepository)
        {
            _entityRepository = airPlaneRepository;
        }

        [HttpPost]
        [Authorize(Policy = "CreateHomeStayAccess")]
        public IActionResult CreateNew(HomeStayModel entityModel)
        {
            try
            {
                var entityExist = _entityRepository.getByName(entityModel.PlaceBranch);

                if (entityExist.Data == null)
                {
                    var response = _entityRepository.Add(entityModel);

                    return Ok(response);
                }
                else
                {
                    var response = new Response
                    {
                        resultCd = 1,
                        MessageCode = "C221",
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
