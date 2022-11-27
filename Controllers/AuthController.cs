using System;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Data.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {
            private readonly IAuthentication _authentication;
            public AuthController(IAuthentication authentication)
            {
                _authentication = authentication;

            }
            

            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] UserRequest userRequest)
            {
                try
                {
                    return Ok(await _authentication.Login(userRequest));
                }
                catch (AccessViolationException)
                {
                    return BadRequest();
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            
    }
}
