namespace ManagementSystem_DotNet8.DTOs
{
    public class AddPermissionRoleDto
    {
       public List<RolePermission> permissions {  get; set; }
    }
    public class RolePermission
    {
        public int Id { get; set; }
        public string roleId { get; set; }
        public string claimType { get; set; }
        public string claimValue { get; set; }
    }
    public class EditRolePermission
    {
        public int Id { get; set; }
        public string claimType { get; set; }
        public bool claimValue { get; set; }
    }
}

