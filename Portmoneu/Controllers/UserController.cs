using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portmoneu.Core.Interfaces;
using Portmoneu.Models.DTO;
using Portmoneu.Models.Helpers;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

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

        /// <summary>
        /// Adding admins
        /// </summary>
        /// <remarks>Admin role needed</remarks>
        /// <param name="adminRegisterDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/add-admin")]
        [Authorize(Policy = "RequireAdminRole")]
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
            var customerid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (customerid != null) { 
                return BadRequest("Please log out to log in");
            }
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

        [HttpGet]
        [Route("api/logout")]
        [AllowAnonymous]
        public async Task<IActionResult> UserLogout() {  
            Response.Cookies.Append(".AspNetCore.Identity.Application", "", new CookieOptions { Expires = DateTime.UtcNow.AddDays(-1) });

            return RedirectToAction("LogoutAction", "User");
        }

        [HttpGet]
        [Route("api/logged-out-message")]
        [AllowAnonymous]
        public async Task<IActionResult> LogoutAction() {
            return Ok("You are now logged out");
        }

        /// <summary>
        /// Add new Customer and User
        /// </summary>
        /// <param name="customerRegisterDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/add-customer")]
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

        /// <summary>
        /// Adds an account to a Customer
        /// </summary>
        /// <remarks>User role required</remarks>
        /// <param name="newAccount"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/add-account")]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> RegisterNewAccount(NewAccountDTO newAccount) {
            var customerid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            
            if (customerid == null) {
                return BadRequest("User identification not found");
            }
            try {
                var result = await _userService.AddNewAccount(newAccount, customerid);
                if (result.Success) { return Ok(result.Data); }
                return BadRequest(result.Message);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// For testing purposes only
        /// </summary>
        /// <returns></returns>
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
