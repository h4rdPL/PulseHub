using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseHub.Core.CustomError;
using PulseHub.Core.Models;
using PulseHub.Infrastructure.Repositories;
using System.Security.Claims;

namespace PulseHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var validCredentials = true;

            if (validCredentials)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email)
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return Ok();
            }
            return Unauthorized();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User cannot be null");
            }

            await _userRepository.Add(user);
            return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, user);
        }



        [HttpDelete("{userId}/delete")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                await _userRepository.Delete(userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User cannot be null");
            }

            try
            {
                await _userRepository.Update(user);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
        }

        [HttpDelete("{userId}/deactivate")]
        public async Task<Result> DeactivateUser(string userId)
        {
            try
            {
                await _userRepository.DeactivateUser(userId);
                return Result.Success();
            }
            catch (KeyNotFoundException ex)
            {
                return Result.Failure(new Error("NotFound", ex.Message)); 
            }
        }


        [HttpPost("{userId}/confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId)
        {
            var result = await _userRepository.ConfirmEmail(userId);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            if (result.Error?.Code == "NotFound")
            {
                return NotFound(result.Error);
            }

            return BadRequest(result.Error);
        }
    }
}
