namespace Backend.Base.Menu
{
    public interface MenuServiceI
    {
        List<int> GetMenuItemIds(SessionEnt session);
    }
}
