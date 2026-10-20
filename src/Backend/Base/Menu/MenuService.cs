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


        public List<int> GetMenuItemIds(SessionEnt session)
        {
            var menuItemIds = new List<int>();

            if (session.IsUserPermission(GC.PerOrg))
                menuItemIds.Add(GC.PerOrg);
            
            if (session.IsUserPermission(GC.PerUser))
                menuItemIds.Add(GC.PerUser);
            
            if (session.IsUserPermission(GC.PerRole))
                menuItemIds.Add(GC.PerRole);
            
            if (session.IsUserPermission(GC.PerLabel))
                menuItemIds.Add(GC.PerLabel);
            
            if (session.IsUserPermission(GC.PerConfig))
                menuItemIds.Add(GC.PerConfig);
            
            if (session.IsUserPermission(GC.PerPerm))
                menuItemIds.Add(GC.PerPerm);
            
            if (session.IsUserPermission(GC.PerAudit))
                menuItemIds.Add(GC.PerAudit);
            
            return menuItemIds;
        }

    }
}
