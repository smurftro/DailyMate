using ApplicationCore.Abstraction;
using Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DailyMate.Controllers
{

    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {

        public IUserService _userservice;
        public UserController(IUserService userservice)
        {
            _userservice = userservice;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(RegisterUserDTOS models)
        {
            var user = await _userservice.Register(models);
            if (user.IsSucces == true)
            {
                return Ok(models);
            }
            return BadRequest();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(LoginUserDtos models)
        {
            var user = await _userservice.Login(models);
            if (user.IsSucces == true)
            {
                return Ok(user);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            return Ok(new
            {
                isAuth = User.Identity?.IsAuthenticated,
                nameId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                name = User.Identity?.Name
            });
        }
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }
    
}
