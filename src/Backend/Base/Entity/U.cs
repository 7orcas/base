/**
 * Utilities
 */
namespace Backend.Base.Entity
{
    public class U
    {
        static public bool IsSameOrg(int? org1, int? org2)
        {
            return CompareOrg(org1, org2) == 0;
        }

        static public int CompareOrg(int? org1, int? org2)
        {
            if (org1 == null && org2 == null) return 0;
            if (org1 == null) return 1;
            if (org2 == null) return -1;
            return org1.Value - org2.Value;
        }

    }
}
