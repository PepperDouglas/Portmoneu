using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portmoneu.Core.Interfaces;
using System.Security.Claims;

namespace Portmoneu.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService) {
            _accountService = accountService;
        }

        [HttpGet]
        [Route("api/account-details")]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> GetAccountDetails() {
            var customer = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (customer == null) {
                return BadRequest("Invalid token");
            }
            try {
                var result = await _accountService.RetrieveAccounts(customer);
                if (result.Success) { 
                    return Ok(result.Data);
                }
                return BadRequest(result.Message);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }


    }
}
