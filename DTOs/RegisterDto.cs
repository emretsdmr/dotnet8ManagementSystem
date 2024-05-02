namespace ManagementSystem_DotNet8.DTOs
{
    public class RegisterDto
    {
        public required string UserName { get; set; }

        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
