using Npgsql;
using Microsoft.Extensions.Caching.Memory;
using GC = Backend.GlobalConstants;

/// <summary>
/// Language management service
/// Created: November 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Label
{
    public class LabelAdminService : BaseService, LabelAdminServiceI
    {

        private readonly LabelAdminRepoI _labelAdminRepo;

        public LabelAdminService(IServiceProvider serviceProvider,
            LabelAdminRepoI labelAdminRepo)
            : base(serviceProvider)
        {
            _labelAdminRepo = labelAdminRepo;
        }
               

        public async Task<List<LangCodeAdmin>> GetLangCodeList(LabelSearch search)
        {
            return await _labelAdminRepo.GetLangCodeList(search);
        }

        public List<LangCodeAdminDto> PopulatCodes(List<LangCodeAdmin> codes)
        {
            var list = new List<LangCodeAdminDto>();

            foreach (var l in codes)
            {
                list.Add(new LangCodeAdminDto
                {
                    Id = l.Id,
                    Code = l.Code,
                    Description = l.Description,
                    Flag = l.Flag
                });
            }

            return list;
        }

        public async Task<List<LangKeyAdmin>> GetLangKeyList(LabelSearch search)
        {
            return await _labelAdminRepo.GetLangKeyList(search);
        }

        public List<LangKeyAdminDto> PopulateKeys(List<LangKeyAdmin> keys)
        {
            var list = new List<LangKeyAdminDto>();

            foreach (var l in keys)
            {
                list.Add(new LangKeyAdminDto
                {
                    Id = l.Id,
                    Code = l.Code,
                    Description = l.Description,
                    Variant = l.Variant
                });
            }

            return list;
        }

        public async Task<List<LangLabelAdmin>> GetLangLabelList(LabelSearch search)
        {
            if (!search.LangCodeId.HasValue)
            {
                _log.Error("Invalid call to GetLangLabelList, LangCodeId can't be null");
                return null;
            }

            List<LangKeyAdmin> keys = await _labelAdminRepo.GetLangKeyList(search);
            List<LangLabelAdmin> labels = await _labelAdminRepo.GetLangLabelList(search);

            //Add missing labels as temp objects
            
            var labelsDic = labels.ToDictionary(p => p.LangKeyId);

            foreach (var key in keys)
            {
                if (!labelsDic.ContainsKey(key.Id))
                    labelsDic[key.Id] = new LangLabelAdmin
                    {
                        Id = GetTempId(),
                        LangCodeId = search.LangCodeId.Value,
                        LangKeyId = key.Id,
                        LangKey = key.Code,
                        Variant = key.Variant,
                        Label = "temp "
                };
            }

            return labelsDic.Values
                            .OrderBy(x => x.LangKey)
                            .ThenBy(x => x.Variant)
                            .ToList();
        }

        public List<LangLabelAdminDto> PopulateLabels(List<LangLabelAdmin> labels)
        {
            var list = new List<LangLabelAdminDto>();

            if (labels == null || labels.Count == 0)
                return list;

            foreach (var l in labels)
            {
                list.Add(new LangLabelAdminDto
                {
                    Id = l.Id,
                    LangCodeId = l.LangCodeId,
                    LangKeyId = l.LangKeyId,
                    LangKey = l.LangKey,
                    Label = l.Label,
                    Variant = l.Variant,
                });
            }

            return list;
        }

        public async Task<LangLabelAdminDto?> NewLangLabel(SessionEnt session, LangLabelNewDto dto)
        {
            //Check if exists
            if (await _labelAdminRepo.IsExist(dto) > 0)
                return null;

            return await _labelAdminRepo.NewLangLabel(dto);
        }

    }
}
