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


        public List<int> GetMenuItemIds(SessionEnt session)
        {
            var menuItemIds = base.GetMenuItemIds(session);
            
            return menuItemIds;
        }

    }
}
