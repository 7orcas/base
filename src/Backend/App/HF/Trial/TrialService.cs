using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using GC = Backend.App.HF.GlobalConstantsHF;

namespace Backend.App.HF.Trial
{
    /// <summary>
    /// Manage loading and processing trials
    /// </summary>
    /// <author>John Stewart</author>
    /// <created>Nov 2026</created>
    /// <license>**Licence**</license>
    public class TrialService: BaseService, TrialServiceI
    {
        private readonly TrialRepoI _trialRepo;
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<TrialHub> _hub;
        private readonly IServiceScopeFactory _scopeFactory;

        public TrialService(IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            TrialRepoI trialRepo,
            IHubContext<TrialHub> hub,
            IServiceScopeFactory scopeFactory) 
            : base(serviceProvider) 
        { 
            _trialRepo = trialRepo;
            _hub = hub;
            _memoryCache = memoryCache;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Return a list of trials
        /// </summary>
        /// <param name="session"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<List<TrialEnt>> GetTrialList(SessionEnt session, TrialSearch search)
        {
            return await _trialRepo.GetList(session, search);
        }

        /// <summary>
        /// Populate the dto's for a list of trials
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public List<TrialDto> Populate(SessionEnt session, List<TrialEnt> entities)
        {
            var list = new List<TrialDto>();
            foreach (var entity in entities)
                list.Add(Populate(session, entity));

            return list;
        }


        /// <summary>
        /// Return the specific trial for the passed in id
        /// </summary>
        /// <param name="trialId"></param>
        /// <returns></returns>
        public async Task<TrialEnt> GetTrialById(long trialId)
        {
            return await _trialRepo.GetById(trialId);
        }

        /// <summary>
        /// Populate a dto for a trial
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TrialDto Populate(SessionEnt session, TrialEnt entity)
        {
            return new TrialDto
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
                0 => session.GetLabel("TrialSt" + status, "Pending"),
                1 => session.GetLabel("TrialSt" + status, "Running"),
                2 => session.GetLabel("TrialSt" + status, "Completed"),
                3 => session.GetLabel("TrialSt" + status, "Failed"),
                _ => session.GetLabel("TrialSt" + status, "Unknown")
            };
        }

        /// <summary>
        /// Get trial configurations for the client
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<TrialConfigDto> GetConfiguration(SessionEnt session)
        {
            var config = new TrialConfigDto();
            config.TrialProgressHubUrl = AppSettings.Urls.Api + GC.TrialProgressHub;
            config.TrialProgressChanged = GC.TrialProgressChanged;
            return config;
        }


        /// <summary>
        /// Get trial field definitions
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<DefinitionDto> GetDefinition(SessionEnt session)
        {
            var validator = new TrialVal(session, _trialRepo);
            return validator.GetDefinition();
        }

        /// <summary>
        /// Validate a list of trial dtos
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public async Task<List<ValidationDto>> ValidateTrials(SessionEnt session, List<TrialDto> update)
        {
            var validator = new TrialVal(session, _trialRepo);
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
        /// Return the specific trial for the passed in dto, or create a new one if it doesn't exist
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrialEnt?> GetTrialOrCreate(SessionEnt session, TrialDto dto)
        {
            //No action required
            if (dto.IsNew() && dto.IsDelete)
                return null;

            if (dto.IsNew())
                return new TrialEnt { 
                    OrgNr = session.OrgNr,
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                };


            return await _trialRepo.GetById(dto.Id);
        }

        /// <summary>
        /// Update, create and delete a Trial
        /// </summary>
        /// <param name="session"></param>
        /// <param name="trial"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TrialEnt> UpdateTrial(SessionEnt session, TrialEnt trial, TrialDto dto)
        {
            return await _trialRepo.Update(session, trial, dto);
        }

        /// <summary>
        /// Get a new Trial entity with default values
        /// Note it is not saved to the database yet, it is just a template for creating a new Trial dto
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<TrialDto?> NewTrial(SessionEnt session)
        {
            var ent = new TrialEnt()
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

        public async Task<TrialInfo> StartTrial(SessionEnt session, TrialDto dto)
        {
            var trial = new TrialInfo
            {
                Id = dto.Id,
                Status = (int)GC.TrialStatus.Running,
                Count = 0
            };
            
            await _trialRepo.UpdateStatus(session, trial.Id, trial.Status);
            dto.Status = trial.Status;
            dto.StatusDescription = GetStatusDescription(session, trial.Status);

            _ = Task.Run(() => RunTrial(session, trial, dto, _memoryCache));

            _memoryCache.Set(Key(trial.Id), trial);
            return trial;
        }

        private async Task RunTrial(SessionEnt session, TrialInfo trial, TrialDto dto, IMemoryCache memoryCache)
        {
            using var scope = _scopeFactory.CreateScope();
            var trialRepo = scope.ServiceProvider.GetRequiredService<TrialRepoI>();

            for (int i = 0; i <= 30; i++)
            {
                trial.Count = i;
                dto.Count = i;

                await _hub.Clients.Group(trial.Id.ToString())
                    .SendAsync(GC.TrialProgressChanged, dto);

                await Task.Delay(500);
            }

            trial.Status = (int)GC.TrialStatus.Completed;
            await trialRepo.UpdateStatus(session, trial.Id, trial.Status);
            memoryCache.Remove(Key(trial.Id));

            dto.Status = trial.Status;
            dto.StatusDescription = GetStatusDescription(session, trial.Status);
            await _hub.Clients.Group(trial.Id.ToString())
                    .SendAsync(GC.TrialProgressChanged, dto);
        }

        public async Task<TrialInfo?> GetTrial(long id)
        {
            if (_memoryCache.TryGetValue(Key(id), out var cachedValue))
            {
                return cachedValue as TrialInfo;
            }
            return null;
        }


        /// <summary>
        /// Populate a dto with trial run status
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Populate(SessionEnt session, TrialInfo info, TrialDto dto)
        {
            dto.Status = info.Status;
            dto.StatusDescription = GetStatusDescription(session, info.Status);
        }


        private string Key(long id)
        {
            return GC.CacheKeyTrialPrefix + id;
        }

    }
}
