using GC = Backend.GlobalConstants;

/// <summary>
/// Manage registration process for new user
/// Created: Jne 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Registration
{
    public class RegistrationService : BaseService, RegistrationServiceI
    {
        private readonly OrgServiceI _orgService;
        private readonly PermissionServiceI _permissionService;

        public RegistrationService(IServiceProvider serviceProvider,
            OrgServiceI orgService,
            PermissionServiceI permissionService) 
            : base (serviceProvider)
        {
            _orgService = orgService;
            _permissionService = permissionService;
        }

       

    }
}
