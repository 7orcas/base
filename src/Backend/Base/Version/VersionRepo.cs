
using Backend.Data;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using GC = Backend.GlobalConstants;

namespace Backend.Base.Version
{
    /// <summary>
    /// Check Version
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>Sept, 2026</created>
    /// <license>**Licence**</license>
    public class VersionRepo : BaseRepo, VersionRepoI
    {
        private readonly AppDbContext _context;

        public VersionRepo(AppDbContext context,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _context = context;
        }

        public async Task<VersionInfo?> GetVersion(long id)
        {
            return await GetVersion(id, "base.zzz");
        }

    }
}
