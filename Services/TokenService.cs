using ManagementSystem_DotNet8.Data;
using ManagementSystem_DotNet8.Entities;
using ManagementSystem_DotNet8.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ManagementSystem_DotNet8.Services
{
    public class TokenService : ITokenService
    {
        readonly IConfiguration configuration;
        private readonly DataContext _context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public TokenService(IConfiguration configuration, DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this._context = context;
            this.roleManager = roleManager;
        }

        public async Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request, User user)
        {
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Secret"]));

            var dateTimeNow = DateTime.Now;

            var claims = new List<Claim>
            {
                new Claim("userName", request.Username)
            };

            var userClaims = await userManager.GetClaimsAsync(user);
     

            var userRoles = await _context.UserRoles.Where(u => u.UserId == user.Id).ToListAsync();
            var roles = await userManager.GetRolesAsync(user);
            
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }

            foreach ( var userRole in userRoles)
            {          
                var role = await _context.Roles.FindAsync(userRole.RoleId);
                var roleClaims = await roleManager.GetClaimsAsync(role);
                claims.AddRange(roleClaims);
            }

            claims.AddRange(userClaims);
           

            JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: configuration["AppSettings:ValidIssuer"],
                    audience: configuration["AppSettings:ValidAudience"],
                    claims: claims,
                    notBefore: dateTimeNow,
                    expires: dateTimeNow.Add(TimeSpan.FromMinutes(20)),
                    signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
                );

            return new GenerateTokenResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                TokenExpireDate = dateTimeNow.Add(TimeSpan.FromMinutes(20)),
            };
        }

     
    }
}
