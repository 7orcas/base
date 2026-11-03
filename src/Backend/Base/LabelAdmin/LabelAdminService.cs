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
               

        public async Task<List<LangCodeAdmin>> GetLangCodeList()
        {
            return await _labelAdminRepo.GetLangCodeList();
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

        public async Task<List<LangKeyAdmin>> GetLangKeyList()
        {
            return await _labelAdminRepo.GetLangKeyList();
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
                });
            }

            return list;
        }

        public async Task<List<LangLabelAdmin>> GetLangLabelList(string langCode)
        {
            return await _labelAdminRepo.GetLangLabelList(langCode);
        }

        public List<LangLabelAdminDto> PopulateLabels(List<LangLabelAdmin> labels)
        {
            var list = new List<LangLabelAdminDto>();

            foreach (var l in labels)
            {
                list.Add(new LangLabelAdminDto
                {
                    Id = l.Id,
                    LangCodeId = l.LangCodeId,
                    LangKeyId = l.LangKeyId,
                    Key = l.Key,
                    Code = l.Code,
                    Variant = l.Variant,
                    Tooltip = l.Tooltip,
                });
            }

            return list;
        }

    }
}
