namespace ManagementSystem_DotNet8.Entities
{
    public class UserLoginResponse
    {
        public bool AuthenticateResult { get; set; } = false;
        public string AuthToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpireDate { get; set; }
        public string userId { get; set; }

   
    }
}
