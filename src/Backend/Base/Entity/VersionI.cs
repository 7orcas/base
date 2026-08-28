namespace Backend.Base.Entity
{
    public interface VersionI
    {
        DateTimeOffset Updated { get; set; }
        int Version { get; set; }
    }
}
