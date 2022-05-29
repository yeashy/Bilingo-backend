using Bilingo.Models;
using Bilingo.Models.UserDTO;
using Bilingo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bilingo.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IRegisterService _registerService;

        public AuthController(ILoginService loginService, IRegisterService register)
        {
            _loginService = loginService;
            _registerService = register;
        }


        [HttpPost]
        [Route("login")]
        public IActionResult Post([FromBody] UserLoginDTO model)
        {
            User? user;
            try
            {
                user = _loginService.LoginUser(model);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
            if (user != null)
            {
                return new JsonResult(_loginService.GetToken(user));
            }
            else
            {
                var response = new
                {
                    message = "Invalid login or password"
                };
                return StatusCode(403, response);
            }
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Post([FromBody] UserRegisterDTO model)
        {
            try
            {
                var token = await _registerService.RegistrateUser(model);
                return new JsonResult(token);
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
        [Route("logout")]
        public IActionResult Post()
        {
            return new JsonResult(new
            {
                message = "OK"
            });
        }
    }
}
