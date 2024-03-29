namespace ManagementSystem_DotNet8.DTOs
{
    public class ResetPasswordDto
    {
        public required string userId { get; set; }
        public required string currentPassword { get; set; }
        public required string newPassword { get; set; }
    }
}
