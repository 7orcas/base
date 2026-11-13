namespace CodeGenerator
{
    public class GlobalConstants 
    {
        public const string BackendAppDs = "DS";
        public const string BackendAppHf = "HF";

        private const string OriginRootPath = "c:/Source/src/";
        private const string OriginBackendPath = OriginRootPath + "Backend/";
        public const string OriginBackendBasePath = OriginBackendPath + "Base/";
        public const string OriginBackendAppPath = OriginBackendPath + "App/";

        private const string DestinationRootPath = "c:/Temp/";
        private const string DestinationBackendPath = DestinationRootPath + "Backend/";
        public const string DestinationBackendAppPath = DestinationBackendPath + "App/";
        public const string DestinationBackendBasePath = DestinationBackendPath + "Base/";
        

        public const string EntityDirectory = "Ent/";
        public const string ControllerPattern = "*Controller.cs";
        public const string ServicePattern = "*Service*";
        public const string RepositoryPattern = "*Repo*";
        public const string UlrsService = "*Repo*";
    }
}
