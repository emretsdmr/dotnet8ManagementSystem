using ManagementSystem_DotNet8.Data;
using ManagementSystem_DotNet8.DTOs;
using ManagementSystem_DotNet8.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ManagementSystem_DotNet8.Controllers
{  
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [Authorize(Policy = "ManageUser")]
        [HttpGet]
        public async Task<ActionResult<List<IdentityRole>>> GetAllRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            if (roles is null)
            {
                return NotFound("Couldn't find any role.");
            }
            return Ok(roles);
        }

        [Authorize(Policy = "ManageRoles")]
        [HttpPost]
        public async Task<ActionResult<List<IdentityRole>>> AddRole(IdentityRole role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return Ok("Role created.");
        }

        [Authorize(Policy = "ManageRoles")]
        [HttpPut]
        public async Task<ActionResult<List<IdentityRole>>> UpdateRole(IdentityRole updatedRole)
        {
            var selectedRole = await _context.Roles.FindAsync(updatedRole.Id);
            if (selectedRole is null)
                return NotFound("Role not found.");
            selectedRole.Name = updatedRole.Name;

            await _context.SaveChangesAsync();
            return Ok(await _context.Roles.ToListAsync());
        }

        [Authorize(Policy = "ManageRoles")]
        [HttpDelete]
        public async Task<ActionResult<List<IdentityRole>>> DeleteRole(string id)
        {
            var selectedRole = await _context.Roles.FindAsync(id);
            if (selectedRole is null)
                return NotFound("Role not found.");

            _context.Roles.Remove(selectedRole);

            await _context.SaveChangesAsync();
            return Ok("Role deleted");
        }

        [Authorize]
        [HttpGet("UserRole")]
        public async Task<ActionResult<List<RolesWithIdsDto>>> GetUserRole(string userId)
        {
            List<RolesWithIdsDto> rolesWithIds = [];
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                return NotFound("User not found.");
            var roles = await userManager.GetRolesAsync(user);
            var roleIds = await _context.UserRoles.Where(u => u.UserId == userId).Select(r => r.RoleId).ToListAsync();
            
            for (int i = 0; i < roles.Count; i++)
            {
                rolesWithIds.Add(new RolesWithIdsDto() { roleName = roles[i],roleId= roleIds[i] });
            }

            return Ok(rolesWithIds);
        }

        [Authorize(Policy = "ManageRoles")]
        [HttpPost("UserRole")]
        public async Task<ActionResult<List<IdentityUserRole<string>>>> AddUserRole(AddUserRoleDto userRole)
        {
            var userWithAllRoles = await _context.UserRoles.Where(x => x.UserId == userRole.userId).ToListAsync();
            var user = await _context.Users.FindAsync(userRole.userId);
            var roles = await _context.Roles.FindAsync(userRole.roleId);
            if (roles is null)
            {
                return NotFound("Role couldn't found!");
            }
            if (user is null)
            {
                return BadRequest("User couldn't found!");
            }
            foreach (var userOneRole in userWithAllRoles)
            {
                if (userOneRole.RoleId == userRole.roleId)
                {
                    return BadRequest("User already has that role!");
                }
            }

            _context.UserRoles.Add(new IdentityUserRole<string>() {UserId=userRole.userId, RoleId=userRole.roleId});
            await _context.SaveChangesAsync();
            return Ok("Role is assigned to user.");

        }

        [Authorize(Policy = "ManageRoles")]
        [HttpDelete("UserRole")]
        public async Task<ActionResult<List<IdentityUserRole<string>>>> DeleteUserRole(IdentityUserRole<string> userRole)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return Ok("Role is deleted from user.");
        }

        [Authorize(Policy = "ManageRoles")]
        [HttpPut("UserRole")]
        public async Task<ActionResult<List<IdentityUserRole<string>>>> UpdateUserRole([FromBody] IdentityUserRole<string> user, string newRole)
        {
            if (user is null)
                return NotFound("User not found.");

            _context.UserRoles.Remove(user);
            _context.UserRoles.Add(new IdentityUserRole<string> { UserId = user.UserId, RoleId = newRole });
            await _context.SaveChangesAsync();
            return Ok("User role updated.");
        }
    }
}
