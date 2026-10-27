namespace Common.DTO.Base
{
    public class RoleDto : _BaseDto
    {
        public List<RolePermissionDto> RolePermissions { get; set; }
    }

    public class RolePermissionDto
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public int PermissionNr { get; set; }
        public string Permission { get; set; }
        public string Crud { get; set; }
        public DateTimeOffset Updated { get; set; }
        public bool IsActive { get; set; }

        //Determined by the permission, if it is an admin permission, then the user must be admin also
        public bool IsEditable { get; set; } 
    }

}
