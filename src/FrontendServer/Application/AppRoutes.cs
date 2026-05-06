using FrontendServer.Base._Base;

namespace FrontendServer.Application
{
    public class AppRoutes : BaseRoutes
    {
        static private Dictionary<string, string> PageCodeDic; //page code, route

        //Routes
        public const string MachinesRoute = "machines";

        //Page Codes
        public const string MachinesPageCode = "mac001";


        private static readonly string[] PageCodeRoutes = {

            MachinesPageCode,       MachinesRoute,
        };


        public static bool IsRoute (string pageCode)
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
                return PageCodeDic[pageCode];
            return null;
        }

        private static void InitialiseRoutes()
        {
            PageCodeDic = new Dictionary<string, string>();
            for (int i = 0; i < BasePageCodeRoutes.Length; i += 2)
            {
                PageCodeDic.Add(BasePageCodeRoutes[i], BasePageCodeRoutes[i + 1]);
            }
            for (int i = 0; i < PageCodeRoutes.Length; i += 2)
            {
                PageCodeDic.Add(PageCodeRoutes[i], PageCodeRoutes[i + 1]);
            }
        }

    }
}
