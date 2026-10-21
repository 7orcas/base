using GC = Backend.GlobalConstants;

/// <summary>
/// Menu items keys
/// Created: Oct 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Backend.App.DS.Base.Menu
{
    public class MenuService : Backend.Base.Menu.MenuService, MenuServiceI
    {
        
        public MenuService(IServiceProvider serviceProvider)
           : base(serviceProvider)  { }


        public List<string> GetMenuItemIds(SessionEnt session)
        {
            var menuItemIds = base.GetMenuItemIds(session);

            if (session.IsUserPermission(GC.PerMachine))
                menuItemIds.Add("Machine");

            return menuItemIds;
        }

    }
}
