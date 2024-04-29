namespace ManagementSystem_DotNet8.DTOs
{
    public class AddPermissionUserDto
    {
        public List<UserPermission> permissions;
        public class UserPermission
        {
            public int Id { get; set; }
            public string userId { get; set; }
            public string claimType { get; set; }
            public string claimValue { get; set; }
        }
        public class EditPermission
        {
            public int Id { get; set; }
            public string claimType { get; set; }
            public bool claimValue { get; set; }
        }
    }
}
