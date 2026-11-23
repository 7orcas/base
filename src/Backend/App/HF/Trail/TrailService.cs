using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using GC = Backend.App.HF.GlobalConstantsHF;

namespace Backend.App.HF.Trail
{
    /// <summary>
    /// Manage loading and processing trails
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>Nov 2026</created>
    /// <license>**Licence**</license>
    public class TrailService: BaseService, TrailServiceI
    {
        private readonly TrailRepoI _trailRepo;
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<TrailHub> _hub;

        public TrailService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            TrailRepoI trailRepo,
            IHubContext<TrailHub> hub) 
            : base(serviceProvider) 
        { 
            _trailRepo = trailRepo;
            _hub = hub;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Return a list of trails
        /// </summary>
        /// <param name="session"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<List<TrailEnt>> GetTrailList(SessionEnt session, TrailSearch search)
        {
            return await _trailRepo.GetList(session, search);
        }

        /// <summary>
        /// Populate the dto's for a list of trails
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public List<TrailDto> Populate(SessionEnt session, List<TrailEnt> entities)
        {
            var list = new List<TrailDto>();
            foreach (var entity in entities)
                list.Add(Populate(session, entity));

            return list;
        }


        /// <summary>
        /// Return the specific trail for the passed in id
        /// </summary>
        /// <param name="trailId"></param>
        /// <returns></returns>
        public async Task<TrailEnt> GetTrailById(long trailId)
        {
            return await _trailRepo.GetById(trailId);
        }

        /// <summary>
        /// Populate a dto for a trail
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TrailDto Populate(SessionEnt session, TrailEnt entity)
        {
            return new TrailDto
            {
                Id = entity.Id,
                Status = entity.Status,
                StatusDescription = GetStatusDescription(session, entity.Status),
                Code = entity.Code,
                Description = entity.Description,
                IsActive = entity.IsActive,
                Version = entity.Version,
                Updated = entity.Updated,
            };
        }

        private string GetStatusDescription(SessionEnt session, int status)
        {
            return status switch
            {
                0 => session.GetLabel("TrailSt" + status, "Pending"),
                1 => session.GetLabel("TrailSt" + status, "Running"),
                2 => session.GetLabel("TrailSt" + status, "Completed"),
                3 => session.GetLabel("TrailSt" + status, "Failed"),
                _ => session.GetLabel("TrailSt" + status, "Unknown")
            };
        }

        /// <summary>
        /// Get trail configurations for the client
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<TrailConfigDto> GetConfiguration(SessionEnt session)
        {
            var config = new TrailConfigDto();
            config.TrailProgressHubUrl = AppSettings.Urls.Api + GC.TrailProgressHub;
            config.TrailProgressChanged = GC.TrailProgressChanged;
            return config;
        }


        /// <summary>
        /// Get trail field definitions
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<DefinitionDto> GetDefinition(SessionEnt session)
        {
            var validator = new TrailVal(session, _trailRepo);
            return validator.GetDefinition();
        }

        /// <summary>
        /// Validate a list of trail dtos
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public async Task<List<ValidationDto>> ValidateTrails(SessionEnt session, List<TrailDto> update)
        {
            var validator = new TrailVal(session, _trailRepo);
            var vals = new List<ValidationDto>();

            foreach (var dto in update)
            {
                if (dto.IsDelete) continue;

                dto.OrgNr = session.OrgNr;

                var val = await validator.Validate(dto);
                if (val != null)
                    vals.Add(val);
            }
            return vals;
        }

        /// <summary>
        /// Return the specific trail for the passed in dto, or create a new one if it doesn't exist
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrailEnt?> GetTrailOrCreate(SessionEnt session, TrailDto dto)
        {
            //No action required
            if (dto.IsNew() && dto.IsDelete)
                return null;

            if (dto.IsNew())
                return new TrailEnt { 
                    OrgNr = session.OrgNr,
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                };


            return await _trailRepo.GetById(dto.Id);
        }

        /// <summary>
        /// Update, create and delete a Trail
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trail"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrailEnt> UpdateTrail(SessionEnt session, TrailEnt trail, TrailDto dto)
        {
            return await _trailRepo.Update(session, trail, dto);
        }

        /// <summary>
        /// Get a new Trail entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new Trail dto
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<TrailDto?> NewTrail(SessionEnt session)
        {
            var ent = new TrailEnt()
            {
                Id = GetTempId(),
                OrgNr = session.Org.Nr,
                Code = "",
                Description = "",
                IsActive = true,
                Version = GC.NewRecordVersion,
            };

            return Populate(session, ent);
        }

        public TrailInfo CreateTrail(TrailDto dto)
        {
            var trail = new TrailInfo
            {
                Id = dto.Id,
                Status = (int)GC.TrailStatus.Running,
                Count = 0
            };

            _ = Task.Run(() => RunTrail(trail, dto));

            _memoryCache.Set(Key(trail.Id), trail);

            return trail;
        }

        public TrailInfo? GetTrail(long id)
        {
            if (_memoryCache.TryGetValue(Key(id), out var cachedValue))
            {
                return cachedValue as TrailInfo;
            }
            return null;
        }

        private async Task RunTrail(TrailInfo trail, TrailDto dto)
        {
            for (int i = 0; i <= 10000; i++)
            {
                trail.Count = i;
                dto.Count = i;

                await _hub.Clients.Group(trail.Id.ToString())
                    .SendAsync(GC.TrailProgressChanged, dto);

                await Task.Delay(500);
            }

            trail.Status = (int)GC.TrailStatus.Completed;
        }

        /// <summary>
        /// Populate a dto with trail run status
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Populate(SessionEnt session, TrailInfo info, TrailDto dto)
        {
            dto.Status = info.Status;
            dto.StatusDescription = GetStatusDescription(session, info.Status);
        }


        private string Key(long id)
        {
            return GC.CacheKeyTrailPrefix + id;
        }

    }
}
