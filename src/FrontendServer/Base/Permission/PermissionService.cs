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
        private readonly IMemoryCache _cache;
        private readonly UrlService _urlService;


        public PermissionService(ProtectedSessionStorage session,
            ConfigService configService,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            UrlService urlService)
        {
            _session = session;
            _configService = configService;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _urlService = urlService;

            if (_cache.TryGetValue(GC.PermCacheKey, out List<PermissionDto> PermissionsX))
                Permissions = PermissionsX;
        }
        public List<UserRolePermissionDto> AllPermissions {  get; private set; }
        public List<PermissionDto> Permissions { get; private set; }

        public PermissionDto? GetPermission(int permissionNr)
        {
            return Permissions.Find(p => p.PermissionNr == permissionNr);
        }

        public async Task ForceInitialise(LoginParameters lps)
        {
            Permissions = null;
            _cache.Remove(GC.PermCacheKey);
            await Initialise(lps);
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
                        _cache.Set(GC.PermCacheKey, Permissions);
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
