using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portmoneu.Core.Interfaces;
using Portmoneu.Models.DTO;
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

        /// <summary>
        /// Lists accounts for the logged customer
        /// </summary>
        /// <remarks>User role required</remarks>
        /// <returns>A list of user Accounts in abridged form</returns>
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

        /// <summary>
        /// Gets transaction details for a single account
        /// </summary>
        /// <remarks>Can only return details if it's the logged customer's account</remarks>
        /// <param name="accountid"></param>
        /// <returns>A list of transaction details</returns>
        [HttpGet]
        [Route("api/account-details-transactions{accountid}")]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> GetAccountTransactionDetails(int accountid) {
            
            var customer = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (customer == null) {
                return BadRequest("Invalid token");
            }
            try {
                var result = await _accountService.GetTransactionDetails(accountid, customer);
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
        /// Transfers money between accounts
        /// </summary>
        /// <remarks>User role required</remarks>
        /// <param name="transactionDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/transfer")]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> TransferMoneyToAccount(TransactionDTO transactionDTO) {
            var customer = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (customer == null) {
                return BadRequest("Invalid token");
            }
            try {
                var result = await _accountService.CreateTransaction(transactionDTO, customer);
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
