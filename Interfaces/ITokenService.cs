using ManagementSystem_DotNet8.Entities;

namespace ManagementSystem_DotNet8.Interfaces
{
    public interface ITokenService
    {
        public Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request, IList<string> roles);


    }
}
