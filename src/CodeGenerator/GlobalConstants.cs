namespace CodeGenerator
{
    public class GlobalConstants 
    {
        public const string BackendAppDs = "DS";
        public const string BackendAppHf = "HF";

        public const string OriginRootPath = "c:/Source/src/";
        public const string OriginBackendPath = OriginRootPath + "Backend/";
        public const string OriginBackendBasePath = OriginBackendPath + "Base/";
        public const string OriginBackendAppPath = OriginBackendPath + "App/";

        public const string DestinationRootPath = "c:/Temp/";
        public const string DestinationBackendPath = DestinationRootPath + "Backend/";
        public const string DestinationBackendAppPath = DestinationBackendPath + "App/";
        public const string DestinationBackendBasePath = DestinationBackendPath + "Base/";
        

        public const string EntityDirectory = "Ent/";
        public const string ControllerPattern = "*Controller.cs";
        public const string ServicePattern = "*Service*";
        public const string RepositoryPattern = "*Repo*";
        public const string UlrsService = "*Repo*";
    }
}
