using Bilingo.Models.UserDTO;
using Bilingo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bilingo.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpDelete]
        [Authorize]
        [Route("deleteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);
            try
            {
                await _userService.DeleteUser(username);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpPatch]
        [Authorize]
        [Route("editUser")]
        public async Task<IActionResult> EditUser([FromBody] UserEditDTO model)
        {
            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);
            try
            {
                await _userService.EditUser(username, model);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("getStatistics")]
        public async Task<IActionResult> GetStatistics()
        {
            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);

            try
            {
                var result = await _userService.GetStatistics(username);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("getUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);
            try
            {
                var result = await _userService.GetUserInfo(username);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("changeAvatar")]
        public async Task<IActionResult> ChangeAvatar([FromForm] IFormFile file)
        {
            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);
            try
            {
                await _userService.ChangeAvatar(username, file);
                return Ok(new { message = "OK" });
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
        }
    }
}
