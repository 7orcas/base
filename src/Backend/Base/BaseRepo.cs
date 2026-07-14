namespace Backend.Base
{
    public abstract class BaseRepo : SqlUtils
    {
        protected readonly Serilog.ILogger _log;

        public BaseRepo(IServiceProvider serviceProvider)
        {
            _log = Log.Logger;
        }
    }
}
