namespace ManagementSystem_DotNet8.DTOs
{
    public class AddPermissionUserDto
    {
        public List<Permission> permissions;
        public class Permission
        {
            public int Id { get; set; }
            public string userId { get; set; }
            public string claimType { get; set; }
            public string claimValue { get; set; }
        }
    }
}
