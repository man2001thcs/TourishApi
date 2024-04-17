using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WebApplication1.Model;
using WebApplication1.Service.InheritanceService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Validate(LoginModel model)
        {
            return Ok(await _userService.Validate(model));
        }

        [HttpPost("CheckExist")]
        public async Task<IActionResult> CheckExist(String userName)
        {
            return Ok(await _userService.CheckExist(userName));
        }

        [HttpPost("GoogleSignIn")]
        public async Task<IActionResult> GoogleSignIn(UserModel model)
        {
            return Ok(await _userService.GoogleSignIn(model));
        }


        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(UserModel model)
        {
            return Ok(await _userService.SignIn(model));
        }

        [Authorize(Policy = "UpdateUserInfoAccess")]
        [HttpPost("Update")]
        public async Task<IActionResult> Update(UserUpdateModel model)
        {
            var bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            return Ok(await _userService.Update(model, bearer_token));
        }

        [Authorize(Policy = "UpdateUserPasswordAccess")]
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(UserUpdatePasswordModel model)
        {
            var bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            return Ok(await _userService.UpdatePassword(model, bearer_token));
        }

        [Authorize(Policy = "GetUserListAccess")]
        [HttpGet("GetUserList")]
        public IActionResult GetUserList(string? search, int type, string? sortBy, int page = 1, int pageSize = 5)
        {
            var bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            return Ok(_userService.GetUserList(bearer_token, search, type, sortBy, page, pageSize));
        }

        [Authorize(Policy = "GetUserAccess")]
        [HttpPost("GetUser")]
        public IActionResult GetUser(Guid id, int type)
        {
            var bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            return Ok(_userService.GetUser(id, type, bearer_token));
        }

        [Authorize(Policy = "SelfGetUserAccess")]
        [HttpPost("SelfGetUser")]
        public IActionResult SelfGetUser()
        {
            var bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            return Ok(_userService.SelfGetUser(bearer_token));
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel tokenModel)
        {
            return Ok(_userService.RenewToken(tokenModel));
        }

        [HttpGet("ValidateSignIn")]
        public async Task<IActionResult> ValidateSignIn(string token)
        {
            var result = await _userService.validateSignInToken(token);

            if (result)
            {
                return Redirect("http://localhost:4200/guest/login");
            }
            else return BadRequest("Token not valid");
        }
    }
}
