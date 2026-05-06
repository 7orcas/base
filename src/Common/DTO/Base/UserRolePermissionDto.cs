namespace Common.DTO.Base
{
    public class UserRolePermissionDto
    {
        public int OrgNr { get; set; }
        public string Role { get; set; }
        public int PermissionNr { get; set; }
        public string LangKey { get; set; }
        public string Crud { get; set; }
    }
}
