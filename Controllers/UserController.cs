using ManagementSystem_DotNet8.Data;
using ManagementSystem_DotNet8.DTOs;
using ManagementSystem_DotNet8.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ManagementSystem_DotNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UserController(DataContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }              


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _context.Users.Include(x => x.Informations).ToListAsync();
            if (users is null)
                return NotFound("Couldn't find any user.");
            return Ok(users);
        }

        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<List<User>>> GetUser(string id)
        {
            var user = await _context.Users.Include(x => x.Informations).FirstOrDefaultAsync(x => x.Id == id);
            if (user is null)
                return NotFound("User not found");
            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<User>>> AddUser(RegisterDto user)
        {
            var newUser = new User() { UserName = user.UserName };
            await userManager.CreateAsync(newUser, user.Password);
            return Ok(user);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<User>>> UpdateUser(string userId, RegisterDto user)
        {
            var newUser = await userManager.FindByIdAsync(userId);
            if (newUser is null)
                return NotFound("User not found");
            var token = await userManager.GeneratePasswordResetTokenAsync(newUser);
            newUser.UserName = user.UserName;
            await userManager.ResetPasswordAsync(newUser, token, user.Password);

            await userManager.UpdateAsync(newUser);
            return Ok("User updated.");
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<User>>> DeleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound("User not found");
            await userManager.DeleteAsync(user);
            return Ok("User deleted");
        }

        

    }
}
