using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portmoneu.Core.Interfaces;
using Portmoneu.Models.DTO;

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


    }
}
