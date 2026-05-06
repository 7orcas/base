namespace Backend.Base.Session
{
    /// <summary>
    /// Return a guaranteed unique session number per call
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>April 5, 2025</created>
    /// <license>**Licence**</license>
    public class UniqueSessionId
    {
        private static int counter = 101;

        public static int GetId()
        {
            return Interlocked.Increment(ref counter);
        }

    }
}
