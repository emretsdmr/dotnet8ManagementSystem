using ManagementSystem_DotNet8.Data;
using ManagementSystem_DotNet8.Entities;
using ManagementSystem_DotNet8.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem_DotNet8.Services
{
    
    public class AuthService : IAuthService
    {
        private readonly ITokenService tokenService;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly DataContext _context;

        public AuthService(ITokenService tokenService, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, UserManager<User> userManager, DataContext context)
        {            
            this.tokenService = tokenService;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this._context = context;
        }
        

        public async Task<UserLoginResponse> LoginUserAsync(UserLoginRequest request)
        {
            UserLoginResponse response = new();

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                throw new ArgumentNullException(nameof(request));
            }
            var user = await userManager.FindByNameAsync(request.Username);
            if(user is null)
            {
                return new UserLoginResponse() { };
            }
            
            var signInResult = await signInManager.PasswordSignInAsync(
                userName: request.Username!,
                password: request.Password,
                isPersistent: false,
                lockoutOnFailure: false);
            if (signInResult.Succeeded)
            {
                var generatedTokenInformation = await tokenService.GenerateToken(new GenerateTokenRequest { Username = request.Username }, user);

                response.AuthenticateResult = true;
                response.AuthToken = generatedTokenInformation.Token;
                response.AccessTokenExpireDate = generatedTokenInformation.TokenExpireDate;                
                response.userId = user.Id;

            }
            return response;
        }
    }
}
