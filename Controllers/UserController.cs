using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Model;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Data;
using Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Model;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
   // [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly IAuthentication _authentication;
        private readonly ImailService _mailService;
        private readonly PasswordServices _passwordSettings;



        public UserController(IUserService userService, IImageService imageService, IAuthentication authentication, ImailService mailService,IOptions<PasswordServices> passwordSettings)
        {
            _userService = userService;
            _imageService = imageService;
            _authentication = authentication;
            _mailService = mailService;
            _passwordSettings = passwordSettings.Value;
        }

      

        [HttpGet]
        [Route("GetById")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserAsyncById(string userId)
        {
            try
            {
                return Ok(await _userService.GetUserById(userId));
            }
            catch (ArgumentNullException argex)
            {
                return BadRequest(argex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegistrationRequest registrationRequest)
        {
            try
            {
                var result = await _authentication.Register(registrationRequest);
                // return CreatedAtAction(nameof(Login), new {Id = result.Id}, result);
                return Created("", result);
            }
            catch (MissingFieldException mex)
            {
                return BadRequest(mex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetByEmail")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserAsyncByEmail(string email)
        {
            try
            {
                return Ok(await _userService.GetUserByEmail(email));
            }
            catch (ArgumentNullException argex)
            {
                return BadRequest(argex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("Get all Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                return Ok(await _userService.GetAllUser());
            }
            catch (Exception)
            {

                return StatusCode(500);
            }
        }


        [HttpPost]
        [Route("AddContact")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAsync(RegistrationRequest registration)

        {
            try
            {
                var result = await _userService.AddNewContact(registration);
                return Created("",registration);

            }
            catch (MissingMemberException mexc)
            {
                return BadRequest(mexc.Message);
            }
            catch (ArgumentException argex)
            {
                return BadRequest(argex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Route("UpdateContact")]
        public async Task<IActionResult> UpdateAsync(UpdateUserRequest request)
        {
            
            try
            {
                var userId = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var result = await _userService.UpdateContact("", request);
                return NoContent();
            }
            catch (MissingMemberException mecx)
            {
                return BadRequest(mecx.Message);
            }
            catch (AggregateException argex)
            {
                return BadRequest(argex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [Route("DeleteContact")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(string userId)
        {
            try
            {

                return Ok(await _userService.DeleteContact(userId));
            }
            catch (MissingMemberException mex)
            {
                return BadRequest(mex.Message);
            }
            catch (ArgumentNullException arg)
            {
                return BadRequest(arg.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPatch]
        [Route("UpdatePhoto")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadPhotoAsync([FromForm] AddImageRequest photoRequest,string userId)
        {
            try
            {
                var result = await _imageService.UploadAsync(photoRequest.Image);
                var check = new ImageAddedDTO()
                {
                    PublicId = result.PublicId,
                    Url = result.Url.ToString()
                };
               
                var userImage = result.Url.ToString();
                var updatePhoto = await _userService.UpdateContactPhoto(userId, userImage);
                return Ok();
            }
            catch (ArgumentException argex)
            {
                return BadRequest(argex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EmailConfirmation([FromForm] MailRequest request)
        {
            try
            {

                await _mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPassword)
        {

            try
            {
                await _passwordSettings.ResetPassword(resetPassword);
                return Ok();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

}
