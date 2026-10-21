namespace Backend.App.DS.Base.Menu
{
    public interface MenuServiceI
    {
        List<string> GetMenuItemIds(SessionEnt session);
    }
}
