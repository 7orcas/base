namespace Backend.Base.Id
{
    public class TempIdService : TempIdServiceI
    {
        private long _lastId = -10;

        public long GetTempId()
        {
            return Interlocked.Decrement (ref _lastId);
        }
    }
}
