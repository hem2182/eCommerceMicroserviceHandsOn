using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Repository;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Response>> RegisterUser(AppUserDTO appUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest($"Invalid Data. {ModelState}");

            var result = await _userRepository.Register(appUserDTO);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest($"Invalid Data. {ModelState}");

            var result = await _userRepository.Login(loginDTO);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserDTO>> GetUser(int id)
        {
            if (id < 0)
                return BadRequest("Invalid input");

            var user = await _userRepository.GetUser(id);
            return user.Id > 0 ? Ok(user) : NotFound(user);
        }
    }
}
