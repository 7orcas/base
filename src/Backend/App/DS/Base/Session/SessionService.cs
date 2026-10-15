using Backend.App.DS.Base.Session.Ent;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.App.DS.Base.Session
{
    public class SessionService : BaseService, SessionServiceI
    {
        public SessionService(IServiceProvider serviceProvider)
            : base(serviceProvider) { }


        public void ConfigureSession(SessionDS session)
        {
            if (session == null) return;
            session.Stations = "STATIONS";
        }
    }
}
