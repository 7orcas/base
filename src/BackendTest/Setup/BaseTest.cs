using Microsoft.Extensions.Caching.Memory;
using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest.Setup
{
    public partial class BaseTest
    {
        public IMemoryCache memoryCache;

        public BaseTest() 
        {
            memoryCache = new MemoryCache(new MemoryCacheOptions());
        }
               

    }
}
