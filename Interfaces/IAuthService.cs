using ManagementSystem_DotNet8.Entities;

namespace ManagementSystem_DotNet8.Interfaces
{
    public interface IAuthService
    {
        public Task<UserLoginResponse> LoginUserAsync(UserLoginRequest request);
    }
}
