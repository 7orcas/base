using Common.DTO;
using Common.Validator;
using Common.Validator.Base;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GC = Backend.GlobalConstants;

namespace Backend.Base
{
    public partial class BaseController
    {
        public async Task<List<ValDto>> Validate<T,E,V>(List<T> dtos, List<E> entsInDb)
             where T : _BaseFieldsDto<T>
             where E : BaseEntity<E>
             where V : ValidatorI<T>, new()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var langDic = await _labelService.GetLangCodeDic(session);

            var valFields = await ValidateFields<T,V>(dtos, session, langDic);
            var valCodesInDB = await ValidateCodesInDB<T,E>(dtos, entsInDb, session, langDic);
            var valCodesNew = await ValidateCodesNew<T>(dtos, session, langDic);
            var valCodesUpdate = await ValidateUpdateDateTime<T,E>(dtos, entsInDb, session, langDic);
            ValidateCombine(valFields, valCodesInDB);
            ValidateCombine(valFields, valCodesNew);
            ValidateCombine(valFields, valCodesUpdate);

            return valFields;
        }

        /// Validate fields within the dtos
        public async Task<List<ValDto>> ValidateFields<T, V>(List<T> dtos)
            where V : ValidatorI<T>, new()
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var langDic = await _labelService.GetLangCodeDic(session);
            return await ValidateFields<T, V>(dtos, session, langDic);
        }

        /// Validate fields within the dtos
        [NonAction]
        public async Task<List<ValDto>> ValidateFields<T, V>(List<T> dtos, SessionEnt session, Dictionary<string, string> langDic)
            where V : ValidatorI<T>, new()
        {
            var validations = new List<ValDto>();
            
            //Check fields
            foreach (var dto in dtos)
            {
                var v = new V().Validate(dto, session.Org.Nr, langDic);
                if (v.Status() != GC.ValStatusOk)
                    validations.Add(v);
            }
            return validations;
        }

        /// Check for duplicate codes between the dtos and entities in the BD <summary>
        public async Task<List<ValDto>> ValidateCodesInDB<T, E>(List<T> dtos, List<E> entsInDb)
             where T : _BaseFieldsDto<T>
             where E : BaseEntity<E>
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var langDic = await _labelService.GetLangCodeDic(session);
            return await ValidateCodesInDB<T, E>(dtos, entsInDb, session, langDic);
        }

        /// Check for duplicate codes between the dtos and entities in the BD <summary>
        [NonAction]
        public async Task<List<ValDto>> ValidateCodesInDB<T, E>(List<T> dtos, List<E> entsInDb, SessionEnt session, Dictionary<string, string> langDic)
             where T : _BaseFieldsDto<T>
             where E : BaseEntity<E>
        {
            var validations = new List<ValDto>();

            // Build a lookup of existing codes to their entities
            var codeLookup = entsInDb
                .Where(e => !string.IsNullOrEmpty(e.Code))
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.ToList());

            var vm = new ValMessage
            {
                Message = GetLabel("InvCE", langDic)
            };

            foreach (var dto in dtos.Where(d => !string.IsNullOrEmpty(d.Code)))
            {
                if (codeLookup.TryGetValue(dto.Code, out var matches))
                {
                    foreach (var ent in matches)
                    {
                        if (ent.Id != dto.Id)
                        {
                            validations.Add(new ValDto
                            {
                                Id = dto.Id,
                                Code = dto.Code,
                                Messages = new List<ValMessage> { vm }
                            });

                            break; // Only need one match to flag it
                        }
                    }
                }
            }

            return validations;
        }

        /// Check for duplicate codes within the dtos
        public async Task<List<ValDto>> ValidateCodesNew<T>(List<T> dtos)
            where T : _BaseFieldsDto<T>
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var langDic = await _labelService.GetLangCodeDic(session);
            return await ValidateCodesNew<T>(dtos, session, langDic);
        }

        /// Check for duplicate codes within the dtos
        [NonAction]
        public async Task<List<ValDto>> ValidateCodesNew<T>(List<T> dtos, SessionEnt session, Dictionary<string, string> langDic)
            where T : _BaseFieldsDto<T>
        {
            var validations = new List<ValDto>();

            // Filter to new items with non-empty codes
            var newDtosWithCode = dtos
                .Where(d => d.IsNew() && !string.IsNullOrWhiteSpace(d.Code))
                .ToList();

            // Group by code to find duplicates
            var duplicateGroups = newDtosWithCode
                .GroupBy(d => d.Code)
                .Where(g => g.Count() > 1);

            var vm = new ValMessage
            {
                Message = GetLabel("InvCD", langDic)
            };

            foreach (var group in duplicateGroups)
            {
                foreach (var dto in group)
                {
                    var val = new ValDto
                    {
                        Id = dto.Id,
                        Code = dto.Code,
                        Messages = new List<ValMessage> { vm }
                    };
                    validations.Add(val);
                }
            }

            return validations;
        }


        /// Check that records in the DB haven't already been changed
        public async Task<List<ValDto>> ValidateUpdateDateTime<T, E>(List<T> dtos, List<E> entsInDb)
            where T : _BaseFieldsDto<T>
            where E : BaseEntity<E>
        {
            var session = HttpContext.Items["session"] as SessionEnt;
            var langDic = await _labelService.GetLangCodeDic(session);
            return await ValidateUpdateDateTime<T, E>(dtos, entsInDb, session, langDic);
        }

        /// Check that records in the DB haven't already been changed
        [NonAction]
        public async Task<List<ValDto>> ValidateUpdateDateTime<T, E>(List<T> dtos, List<E> entsInDb, SessionEnt session, Dictionary<string, string> langDic)
            where T : _BaseFieldsDto<T>
            where E : BaseEntity<E>
        {
            var validations = new List<ValDto>();

            // Build a lookup of existing codes to their entities
            var entLookup = entsInDb.ToDictionary(g => g.Id, g => g);

            foreach (var dto in dtos)
            {
                if (dto.IsNew()) continue;
                var ent = entLookup[dto.Id];
                if (ent.Updated <= dto.Updated) continue;

                var vm = new ValMessage
                {
                    Message = GetLabel("InvUD", langDic).Replace(GC.LabelParameterPrefix, ent.Updated.ToString("yyyy-MM-dd HH:mm:ss"))
                };

                var val = new ValDto
                {
                    Id = dto.Id,
                    Code = dto.Code,
                    Messages = new List<ValMessage> { vm }
                };
                validations.Add(val);
            }

            return validations;
        }


        [NonAction]
        public void ValidateCombine(List<ValDto> into, List<ValDto> from)
        {
            var intoMap = into.ToDictionary(v => v.Id);

            foreach (var valX in from)
            {
                if (intoMap.TryGetValue(valX.Id, out var existing))
                {
                    existing.Messages.AddRange(valX.Messages);
                }
                else
                {
                    into.Add(valX);
                    intoMap[valX.Id] = valX;
                }
            }
        }

    }
}
