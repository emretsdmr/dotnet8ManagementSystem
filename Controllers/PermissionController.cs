using ManagementSystem_DotNet8.Data;
using ManagementSystem_DotNet8.DTOs;
using ManagementSystem_DotNet8.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.Data;
using System.Data.OleDb;
using System.Net;
using System.Security.Claims;
using System.Xml.Linq;
using static ManagementSystem_DotNet8.DTOs.AddPermissionUserDto;

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


        [HttpGet("getUserPolicies")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Claim>>> GetUserPolicies(string userId)
        {
            var permissions = await _context.UserClaims.Where(u => u.UserId == userId).ToListAsync();
            if (permissions is null)
                return NotFound("Couldn't find any permissions.");
            return Ok(permissions);
        }

        [HttpPut("editUserPolicy")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<IdentityUserClaim<string>>>> EditUserPolicy(List<UserPermission> newPermissions)
        {
            //veritabanından policyleri çek
            //newPermissions ile veritabanından gelen claimValue'ları kontrol et
            var user = await userManager.FindByIdAsync(newPermissions[0].userId);
            var oldPermissions = await _context.UserClaims.Where(u => u.UserId == user.Id).ToListAsync();            

            
            if(newPermissions.Count > oldPermissions.Count)
            {
                var policiesToAdd = newPermissions.ExceptBy(oldPermissions.Select(o => o.ClaimType), n => n.claimType);                
                foreach (var policy in policiesToAdd)
                {
                    if (policy.claimValue == "True")
                    {
                        await userManager.AddClaimAsync(user, new Claim(policy.claimType, policy.claimValue));
                        
                    }
                }
                return Ok("permission added");
            }
            if(newPermissions.Count < oldPermissions.Count)
            {
                var policiesToDelete = oldPermissions.ExceptBy(newPermissions.Select(n => n.claimType), o => o.ClaimType);
                
                foreach(var policy in policiesToDelete)
                {                                                             
                        _context.UserClaims.Remove(policy);
                        await _context.SaveChangesAsync();

                }
                return Ok("permission deleted");
            }
            /*if (newPermissions.Count == oldPermissions.Count)
            {                    
                foreach (var policy in oldPermissions)
                {
                    if(policy.ClaimValue == "False")
                    {
                        var policyX = _context.UserClaims.FindAsync(policy.Id);
                        _context.UserClaims.Remove(policy);
                        await userManager.RemoveClaimsAsync(user, policy);
                    }
                        
                    
                    
                    await _context.SaveChangesAsync();

                }
                return Ok("permission deleted");
            }*/

            return Ok("ok");
            
        }

        [HttpPost("addRolePolicy")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<IdentityUserClaim<string>>>> AddPolicyToRole(List<RolePermission> permissions)
        {
            foreach (var permission in permissions)
            {
                var role = await roleManager.FindByIdAsync(permission.roleId);
                await roleManager.AddClaimAsync(role, new Claim(permission.claimType, permission.claimValue));
            }


            return Ok("permission added");
        }

        [HttpGet("getRolePolicies")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<IdentityRoleClaim<string>>>> GetRolePolicies(string roleId)
        {
            var roleClaims = await _context.RoleClaims.Where(r => r.RoleId == roleId).ToListAsync();
            return Ok(roleClaims);
        }

        [HttpPut("updateRolePolicies")]
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
                        policy.ClaimType = permission.claimType;
                        policy.ClaimValue = permission.claimValue.ToString();
                    }
                    
                }                
            }

            await _context.SaveChangesAsync();
            return Ok("permission updated");
        }      

    }
}
