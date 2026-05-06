using Microsoft.Extensions.Caching.Memory;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest
{
    public partial class BaseTest
    {
        private static bool initialisedDb = false;
        public readonly IMemoryCache memoryCache;
        

        public BaseTest() 
        {
            AppSettings.DBMainConnection = GCT.ConnString;
            memoryCache = new MemoryCache(new MemoryCacheOptions());
            InitialiseServices();
        }

        //Will run each time for all tests
        //[TestInitialize]
        //public static async Task InitialiseDb(TestContext context)
        //{
        //    ResetInitialisedDb();
        //    await SetupTestDb();
        //}

        public static void ResetInitialisedDb() => initialisedDb = false;
        public bool IsInitialisedDb() => initialisedDb;


    }
}
