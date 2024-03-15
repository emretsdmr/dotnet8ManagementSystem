using ManagementSystem_DotNet8.Data;
using ManagementSystem_DotNet8.DTOs;
using ManagementSystem_DotNet8.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem_DotNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public LoginController(DataContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] RegisterDto user)
        {
            var signInResult = await signInManager.PasswordSignInAsync(
                userName: user.UserName!,
                password: user.Password,
                isPersistent: false,
                lockoutOnFailure: false);
            if (signInResult.Succeeded)
            {
                var response = await userManager.GetUserAsync(User);
                return Ok(response.Id);
            }
            return BadRequest("Error occured");
        }
    }
}
