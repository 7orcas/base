using GC = Backend.GlobalConstants;

/// <summary>
/// Menu items keys
/// Created: Oct 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.Base.Menu
{
    public class MenuService : BaseService, MenuServiceI
    {
        
        public MenuService(IServiceProvider serviceProvider)
           : base(serviceProvider)  { }


        public List<string> GetMenuItemIds(SessionEnt session)
        {
            var menuItemIds = new List<string>();

            if (session.IsUserPermission(GC.PerOrg))
                menuItemIds.Add("Org");
            
            if (session.IsUserPermission(GC.PerUser))
                menuItemIds.Add("User");
            
            if (session.IsUserPermission(GC.PerRole))
                menuItemIds.Add("Role");

            if (session.IsUserPermission(GC.PerLabel))
                menuItemIds.Add("Label");
            
            if (session.IsUserPermission(GC.PerConfig))
                menuItemIds.Add("Config");
            
            if (session.IsUserPermission(GC.PerPerm))
                menuItemIds.Add("Perm");
            
            if (session.IsUserPermission(GC.PerAudit))
                menuItemIds.Add("Audit");
            
            return menuItemIds;
        }

    }
}
