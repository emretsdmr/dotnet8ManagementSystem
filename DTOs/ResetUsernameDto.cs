namespace ManagementSystem_DotNet8.DTOs
{
    public class ResetUsernameDto
    {
        public required string userId { get; set; }
        public required string newUsername { get; set; }
    }
}
