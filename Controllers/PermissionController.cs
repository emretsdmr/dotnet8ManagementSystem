using ManagementSystem_DotNet8.Data;
using ManagementSystem_DotNet8.DTOs;
using ManagementSystem_DotNet8.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Data;
using System.Security.Claims;

namespace ManagementSystem_DotNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public PermissionController(DataContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }


        /*[HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Claim>>> GetAllUserPermissions()
        {
            var permissions = await _context.UserClaims.ToListAsync();
            if (permissions is null)
                return NotFound("Couldn't find any permissions.");
            return Ok(permissions);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<IdentityUserClaim<string>>>> AddPermissionToUser(AddPermissionDto permission)
        {
            var user = await userManager.FindByIdAsync(permission.userId);

            await userManager.AddClaimAsync(user, new Claim(permission.claimType, permission.claimValue));

            return Ok("permission added");
        }*/

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<IdentityUserClaim<string>>>> AddPolicyToRole(List<Permission> permissions)
        {
            foreach (var permission in permissions)
            {
                var role = await roleManager.FindByIdAsync(permission.roleId);
                await roleManager.AddClaimAsync(role, new Claim(permission.claimType, permission.claimValue));
            }


            return Ok("permission added");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<IdentityRoleClaim<string>>>> GetRolePolicies(string roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            var roleClaim = await roleManager.GetClaimsAsync(role);
            return Ok(roleClaim);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<IdentityUserClaim<string>>>> EditRolePolicy(string roleId, List<EditPermission> permissions)
        {
            var role = await _context.Roles.FindAsync(roleId);
            foreach (var permission in permissions)
            {
                var policies = await _context.RoleClaims.Where(r => r.RoleId == roleId).ToListAsync();
                //var policies = await roleManager.GetClaimsAsync(role);
                foreach (var policy in policies)
                {
                    if(policy.Id == permission.Id)
                    {                        
                        policy.ClaimValue = permission.claimValue;
                    }
                    
                }                
            }

            await _context.SaveChangesAsync();
            return Ok("permission updated");
        }
    }
}
