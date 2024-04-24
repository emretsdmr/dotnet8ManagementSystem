using ManagementSystem_DotNet8.Entities;
using System.Security.Claims;

namespace ManagementSystem_DotNet8.Interfaces
{
    public interface ITokenService
    {
        public Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request, User user);


    }
}
