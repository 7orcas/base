namespace Common.DTO.Base
{
    public class RoleDto : _BaseFieldsDto<RoleDto>
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
        public DateTime Updated { get; set; }
        public bool IsActive { get; set; }
    }

}
