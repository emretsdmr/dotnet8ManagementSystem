namespace ManagementSystem_DotNet8.DTOs
{
    public class AddPermissionRoleDto
    {
       public List<Permission> permissions {  get; set; }
    }
    public class Permission
    {
        public int Id { get; set; }
        public string roleId { get; set; }
        public string claimType { get; set; }
        public string claimValue { get; set; }
    }
    public class EditPermission
    {
        public int Id { get; set; }
        public string claimValue { get; set; }
    }
}

