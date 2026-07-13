using Backend.Base.Mfa.Ent;

namespace Backend.Base.Mfa
{
    public interface MfaServiceI
    {
        Task<MfaSetup?> SetupMfa(long id, string username);
        Task<bool> VerifyMfaCode(long id, string mfaCode);
    }
}
