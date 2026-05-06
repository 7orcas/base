namespace Backend.Base.Role.Ent
{
    public class RolePermissionEnt
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public int PermissionNr { get; set; }
        public string Crud { get; set; }
        public DateTime Updated { get; set; }
        public bool IsActive { get; set; }
    }
}
