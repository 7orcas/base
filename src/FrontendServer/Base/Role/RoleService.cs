using System.Collections.Generic;
using System.Net.Http;
using Common.DTO.Base;
using FrontendServer.Base._Base;
using FrontendServer.Base.Cache;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using GC = FrontendServer.GlobalConstants;

namespace FrontendServer.Base.Role
{
    public class RoleService : BaseServiceDELETE_ME
    {
        public RoleService(LabelCacheService Cache) : base (Cache) { }

        public async Task<(List<UserRoleDto> roles, MarkupString message)> RolesAsync(HttpClient client)
        {
            return await GetAsync<List<UserRoleDto>>(client, GC.URL_user_role_list);
        }

    }
}
