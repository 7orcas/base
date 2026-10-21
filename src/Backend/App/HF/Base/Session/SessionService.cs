using Backend.App.HF.Base.Session.Ent;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.App.HF.Base.Session
{
    public class SessionService : BaseService, SessionServiceI
    {
        public SessionService(IServiceProvider serviceProvider)
            : base(serviceProvider) { }


        public void ConfigureSession(SessionEntHF session)
        {
            if (session == null) return;
            session.Trails = "TRAILS";
        }
    }
}
