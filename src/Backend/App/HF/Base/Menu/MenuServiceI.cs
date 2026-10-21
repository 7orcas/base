namespace Backend.App.HF.Base.Menu
{
    public interface MenuServiceI
    {
        List<string> GetMenuItemIds(SessionEnt session);
    }
}
