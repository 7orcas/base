using System.Collections.Generic;
using System.Net.Http;
using Common.DTO.Base;
using FrontendServer.Base._Base;
using Newtonsoft.Json;
using GC = FrontendServer.GlobalConstants;

namespace FrontendServer.Base.Audit
{

    //DELETE ME

    public class AuditService : BaseServiceDELETE_ME
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuditService(IHttpClientFactory httpClientFactory, LabelCacheService Cache) : base(Cache) 
        {
            _httpClientFactory = httpClientFactory;
        }


        public async Task<(List<AuditDto> list, string message)> AuditListAsync(string token)
        {
            var client = _httpClientFactory.CreateClient(GC.AuthorizedClientKey);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(GC.BearerKey, token);

            var list = new List<AuditDto>();
            var message = "";

            var response = await client.GetAsync(GC.URL_audit_list);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<_ResponseDto>(result);

                if (responseDto.Valid)
                    list = JsonConvert.DeserializeObject<List<AuditDto>>(responseDto.Result.ToString());
                else
                    message = responseDto.ErrorMessage;


                return (list, message);
            }

            //ToDo label
            return (list, "Opps, something went wrong?");
        }

    }
}
