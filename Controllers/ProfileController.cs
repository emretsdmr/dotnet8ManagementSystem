using ManagementSystem_DotNet8.Data;
using ManagementSystem_DotNet8.DTOs;
using ManagementSystem_DotNet8.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem_DotNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public ProfileController(DataContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        [HttpPut("resetPassword")]
        [Authorize]
        public async Task<ActionResult<List<User>>> ResetPassword(ResetPasswordDto user)
        {
            var newUser = await userManager.FindByIdAsync(user.userId);
            if (newUser is null) 
            { 
                return BadRequest("User couldn't found!"); 
            }
            if (await userManager.CheckPasswordAsync(newUser, user.currentPassword))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(newUser);
                await userManager.ResetPasswordAsync(newUser, token, user.newPassword);
                return Ok(newUser);

            }
            else
            {
                return BadRequest("Current password is wrong!");
            }
        }

        [HttpPut("resetUsername")]
        [Authorize]
        public async Task<ActionResult<List<User>>> ResetUsername(ResetUsernameDto user)
        {
            var newUser = await userManager.FindByIdAsync(user.userId);
            if (newUser is null) 
            { 
                return BadRequest("User couldn't found!"); 
            }
            await userManager.SetUserNameAsync(newUser,user.newUsername);
            return Ok("Username changed!");

            
        }
    }
}
