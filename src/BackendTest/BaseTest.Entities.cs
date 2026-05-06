using GC = Backend.GlobalConstants;
using GCT = BackendTest.GlobalConstants;

namespace BackendTest
{
    public partial class BaseTest
    {
        public async Task<OrgEnt> GetOrgEnt()
        {
            var org = await orgService.GetOrg(GCT.OrgNr);
            return org;
        }


        //public async Task<UserEnt> GetUserEnt()
        //{
        //    var result = await loginservice.GetLogin(GCT.UserName, GCT.OrgNr);
        //    var org = await orgService.GetOrg(GCT.OrgNr);
        //    var user = await loginservice.InitialiseLogin(result.login, org, GC.AppClient);
        //    return user;
        //}

    }
}
