using ManagementSystem_DotNet8.DTOs;
using ManagementSystem_DotNet8.Entities;
using ManagementSystem_DotNet8.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem_DotNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IAuthService authService;
        private readonly UserManager<User> userManager;

        public AuthController(IAuthService authService, UserManager<User> userManager)
        {
            this.authService = authService;
            this.userManager = userManager;
        }

        [HttpPost("LoginUser")]
        [AllowAnonymous]
        public async Task<ActionResult<UserLoginResponse>> LoginUserAsync([FromBody] UserLoginRequest request)
        {
            var result = await authService.LoginUserAsync(request);



            return result;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterDto register)
        {
            var user = new User() { Email = register.Email, UserName=register.UserName };
            var passwordValidator = new PasswordValidator<User>();
            var result = await passwordValidator.ValidateAsync(userManager, null, register.Password);

            if (result.Succeeded)
            {
                await userManager.CreateAsync(user, register.Password);
                return Ok("register succeeded");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
    }
}
