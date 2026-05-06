using Npgsql;
using Microsoft.Extensions.Caching.Memory;
using GC = Backend.GlobalConstants;

/// <summary>
/// Organisation methods
/// Created: March 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>


namespace Backend.Base.Org
{
    public class OrgService: BaseService, OrgServiceI
    {
        private readonly IMemoryCache _memoryCache;
        
        public OrgService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache) 
            : base(serviceProvider) 
        {
            _memoryCache = memoryCache;
        }

        public async Task<List<OrgEnt>> GetOrgList()
        {
            var list = new List<OrgEnt>();
            await Sql.Run(
                    "SELECT * FROM base.org ",
                    r => {
                        var org = new OrgEnt();
                        org.Nr = GetInt(r, "nr");
                        org.Code = GetCode(r);
                        org.Description = GetDescription(r);
                        org.Updated = GetUpdated(r);
                        org.IsActive = IsActive(r);
                        list.Add(org);
                    }
                );
            return list;
        }

        public async Task<OrgEnt> GetOrg(int nr)
        {
            var org = _memoryCache.Get<OrgEnt>(GC.CacheKeyOrgPrefix + nr);
            if (org != null) return org;

            try
            {
                await Sql.Run(
                    "SELECT * FROM base.org "
                    + "WHERE nr = @nr ",
                    r => {
                        org = OrgLoad.Load(r);
                        _memoryCache.Set(GC.CacheKeyOrgPrefix + org.Nr, org);
                    },
                    new NpgsqlParameter("@nr", nr)
                );
                                
                return org;
            }
            catch 
            {
                return null;
            }
        }

        public async Task UpdateOrg(OrgEnt org)
        {
            org.Encode();
            await Sql.Execute(
                    "UPDATE base.org " +
                    "SET " +
                        Update("code", org.Code) +
                        Update("descr", org.Description) +
                        Update("encoded", org.Encoded) +
                        Update("updated", org.Updated) +
                        Update("isActive", org.IsActive) +
                        Update("langCode", org.LangCode) +
                        NoComma(Update("langLabelVariant", org.LangLabelVariant)) +
                    " WHERE nr = " + org.Nr
            );
            _memoryCache.Set(GC.CacheKeyOrgPrefix + org.Nr, org);
        }

    }
}
