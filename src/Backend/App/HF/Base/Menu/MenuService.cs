using GC = Backend.GlobalConstants;

/// <summary>
/// Menu items keys
/// Created: Oct 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.App.HF.Base.Menu
{
    public class MenuService : Backend.Base.Menu.MenuService, MenuServiceI
    {
        
        public MenuService(IServiceProvider serviceProvider)
           : base(serviceProvider)  { }


        public List<string> GetMenuItemIds(SessionEnt session)
        {
            var menuItemIds = base.GetMenuItemIds(session);

            if (session.IsUserPermission(GC.PerCodeTemplate))
                menuItemIds.Add("CodeTemplate");

            if (session.IsUserPermission(GC.PerTrail)) menuItemIds.Add("Trail");
            if (session.IsUserPermission(GC.PerTrailGroup)) menuItemIds.Add("TrailGroup");
            //INSERT_GENERATED_CODE_HERE

            return menuItemIds;
        }

    }
}
