using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portmoneu.Core.Interfaces;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Helpers;

namespace Portmoneu.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) {
            _userService = userService;
        }

        //temporary test api route
        [HttpPost]
        [Route("api/add-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin(AdminRegisterDTO adminRegisterDTO) {
            try {
                var result = await _userService.RegisterAdmin(adminRegisterDTO);
                if (result.Success) {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/login")]
        [AllowAnonymous]
        public async Task<IActionResult> UserLogin(UserCredentials credentials) {
            try {
                var result = await _userService.UserLogin(credentials);
                if (result.Success) {
                    return Ok(result.Data); //which is the token
                }
                return Unauthorized(result.Message);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/add-customer")]
        //no
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> RegisterCustomer(CustomerRegisterDTO customerRegisterDTO) {
            try {
                var result = await _userService.RegisterCustomer(customerRegisterDTO);
                if (result.Success) {
                    return Ok(result.Data);
                }
                return BadRequest(result.Message);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/tokentest")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DoesHaveValidAdminToken() {
            var customerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            if (User.IsInRole("Admin")) {
                return Ok("You are an admin");

            }
            return Ok("You are not an admin, and should not be in here");
        }

    }
}
