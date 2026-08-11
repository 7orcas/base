namespace Backend.Base
{
    public abstract class BaseRepo : SqlUtils
    {
        protected readonly Serilog.ILogger _log;

        public BaseRepo(IServiceProvider serviceProvider)
        {
            _log = Log.Logger;
        }

        protected void VersionIncrement(VersionI entity)
        {
            if (entity.Version == null)
                entity.Version = 0;
            
            entity.Version++;
            entity.Updated = DateTimeOffset.UtcNow;
        }

    }
}
