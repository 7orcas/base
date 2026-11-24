using FrontendServer.Base._Base;
using GC = FrontendServer.GlobalConstants;

namespace FrontendServer.Application
{
    public class AppRoutes : BaseRoutes
    {
        static private Dictionary<string, AppRoute> PageCodeDic; //page code, route

        //Routes
        public const string MachinesRoute = "machines";
        public const string CodeTemplateRoute = "code-templates";

        public const string TrialRoute = "trials";
        public const string TrialGroupRoute = "trialGroups";
        //INSERT_GENERATED_ROUTE_HERE


        //Page Codes
        public const string MachinesPageCode = "mac001";
        public const string CodeTemplatePageCode = "cod001";

        public const string TrialPageCode = "tri001";
        public const string TrialGroupPageCode = "trg001";
        //INSERT_GENERATED_PAGE_CODE_HERE

        private static readonly object[] PageCodeRoutes = {

            MachinesPageCode,       MachinesRoute,    GC.PerMachine,

            CodeTemplatePageCode,   CodeTemplateRoute, GC.PerCodeTemplate,

            TrialPageCode, TrialRoute,  GC.PerTrial,
            TrialGroupPageCode, TrialGroupRoute,  GC.PerTrialGroup,
            //INSERT_GENERATED_PAGE_CODE_ROUTE_HERE
        };


        public static bool IsRoute(string pageCode)
        {
            if (PageCodeDic == null)
                InitialiseRoutes();
            return PageCodeDic.ContainsKey(pageCode);
        }

        public static string? GetRoute(string pageCode)
        {
            if (PageCodeDic == null)
                InitialiseRoutes();
            if (IsRoute(pageCode))
                return PageCodeDic[pageCode].Route;
            return null;
        }

        public static int? GetPermissionNr(string pageCode)
        {
            if (PageCodeDic == null)
                InitialiseRoutes();
            if (IsRoute(pageCode))
                return PageCodeDic[pageCode].PermissionNr;
            return null;
        }

        private static void InitialiseRoutes()
        {
            PageCodeDic = new Dictionary<string, AppRoute>();
            for (int i = 0; i < BasePageCodeRoutes.Length; i += MaxFields)
            {
                PageCodeDic.Add(
                    BasePageCodeRoutes[i + PageCodeIndex].ToString(),
                    new AppRoute(
                        BasePageCodeRoutes[i + PageCodeIndex].ToString(),
                        BasePageCodeRoutes[i + RouteIndex].ToString(),
                        (int)BasePageCodeRoutes[i + PermissionIndex]
                    )
                );
            }
            for (int i = 0; i < PageCodeRoutes.Length; i += MaxFields)
            {
                PageCodeDic.Add(
                    PageCodeRoutes[i + PageCodeIndex].ToString(),
                    new AppRoute(
                        PageCodeRoutes[i + PageCodeIndex].ToString(),
                        PageCodeRoutes[i + RouteIndex].ToString(),
                        (int)PageCodeRoutes[i + PermissionIndex]
                    )
                );
            }
        }
    }

    public class AppRoute
    {
        public string PageCode { get; set; }
        public string Route { get; set; }
        public int PermissionNr { get; set; }

        public AppRoute(string pageCode, string route, int permissionNr)
        {
            PageCode = pageCode;
            Route = route;
            PermissionNr = permissionNr;
        }

    }
}
