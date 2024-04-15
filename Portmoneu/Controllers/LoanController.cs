using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portmoneu.Core.Interfaces;
using Portmoneu.Data.Interfaces;
using Portmoneu.Models.DTO;

namespace Portmoneu.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService) {
            _loanService = loanService;
        }


        /// <summary>
        /// Gives a loan to a Customer
        /// </summary>
        /// <remarks>Admin role required</remarks>
        /// <param name="loanDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/set-loan")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> RegisterLoan(NewLoanDTO loanDTO) {
            try {
                var result = await _loanService.CreateLoan(loanDTO);
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
