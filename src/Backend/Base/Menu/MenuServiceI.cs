namespace Backend.Base.Menu
{
    public interface MenuServiceI
    {
        List<string> GetMenuItemIds(SessionEnt session);
    }
}
