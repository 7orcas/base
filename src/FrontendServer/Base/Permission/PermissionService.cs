using Common.DTO.Base;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using GC = FrontendServer.GlobalConstants;

/// <summary>
/// Permission and CRUD settings
/// Created: July 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendServer.Base.Permission
{
    public class PermissionService : BaseService
    {
        public List<UserRolePermissionDto> AllPermissions {  get; private set; }
        public List<PermissionDto> Permissions { get; private set; }
        private readonly UrlService _urlService;


        public PermissionService(ProtectedSessionStorage session,
            ConfigService configService,
            IHttpClientFactory httpClientFactory,
            UrlService urlService)
        {
            _session = session;
            _configService = configService;
            _httpClientFactory = httpClientFactory;
            _urlService = urlService;
        }

        public PermissionDto? GetPermission(int permissionNr)
        {
            return Permissions.Find(p => p.PermissionNr == permissionNr);
        }

        public async Task Initialise(LoginParameters lps)
        {
            try
            {
                if (!_configService.IsInitialized)
                    await _configService.Initialise();

                var config = _configService.Config;
                var client = await GetClient();

                var response = await client.GetAsync(_urlService.GetUrl("PermissionUserList"));
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var responseDto = JsonConvert.DeserializeObject<_ResponseDto>(result);

                    if (responseDto.Valid)
                    {
                        AllPermissions = JsonConvert.DeserializeObject<List<UserRolePermissionDto>>(responseDto.Result.ToString());
                        lps.LoadedUrl(_urlService.GetUrl("PermissionUserList"));
                    }
                }
            }
            catch
            {
            }

            try
            {
                if (!_configService.IsInitialized)
                    await _configService.Initialise();

                var config = _configService.Config;
                var client = await GetClient();

                var response1 = await client.GetAsync(_urlService.GetUrl("PermissionUserListEff"));
                if (response1.IsSuccessStatusCode)
                {
                    var result = await response1.Content.ReadAsStringAsync();
                    var responseDto = JsonConvert.DeserializeObject<_ResponseDto>(result);

                    if (responseDto.Valid)
                    {
                        Permissions = JsonConvert.DeserializeObject<List<PermissionDto>>(responseDto.Result.ToString());
                        lps.LoadedUrl(_urlService.GetUrl("PermissionUserListEff"));
                    }
                }
            }
            catch
            {
            }
        }

    }
}
