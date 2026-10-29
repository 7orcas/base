using Common.DTO.Base;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Caching.Memory;
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

        public PermissionService(ProtectedSessionStorage session,
            IHttpClientFactory httpClientFactory,
            ConfigService configService,
            UrlService urlService)
        {
            _session = session;
            _httpClientFactory = httpClientFactory;
            _configService = configService;
            _urlService = urlService;
        }

        public async Task InitializeFromCache()
        {
            if (Permissions != null) return;

            var result = await _session.GetAsync<List<PermissionDto>>(GC.PermCacheKey);

            if (result.Success)
                Permissions = result.Value;
        }

        public List<UserRolePermissionDto> AllPermissions {  get; private set; }
        public List<PermissionDto> Permissions { get; private set; }

        public PermissionDto? GetPermission(int permissionNr)
        {
            return Permissions.Find(p => p.PermissionNr == permissionNr);
        }

        public bool IsPermission(int permissionNr)
        {
            //Check if the permission exists in the list of permissions
            return GetPermission(permissionNr) != null;
        }

        public async Task Initialise(LoginParameters lps)
        {
            try
            {
                if (!_configService.IsInitialized)
                    await _configService.Initialise();

                var config = _configService.Config;
                var client = await GetClient();

                var url = _urlService.GetUrl("PermissionUserList");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var responseDto = JsonConvert.DeserializeObject<_ResponseDto>(result);

                    if (responseDto.Valid)
                    {
                        AllPermissions = JsonConvert.DeserializeObject<List<UserRolePermissionDto>>(responseDto.Result.ToString());
                        lps.LoadedUrl(url);
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

                var url = _urlService.GetUrl("PermissionUserListEff");
                var response1 = await client.GetAsync(url);
                if (response1.IsSuccessStatusCode)
                {
                    var result = await response1.Content.ReadAsStringAsync();
                    var responseDto = JsonConvert.DeserializeObject<_ResponseDto>(result);

                    if (responseDto.Valid)
                    {
                        Permissions = JsonConvert.DeserializeObject<List<PermissionDto>>(responseDto.Result.ToString());
                        await _session.SetAsync(GC.PermCacheKey, Permissions);
                        lps.LoadedUrl(url);
                    }
                }
            }
            catch
            {
            }
        }

    }
}
