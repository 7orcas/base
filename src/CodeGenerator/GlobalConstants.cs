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

        public const string DestinationRootPath = "c:/Source/src/"; //"c:/Temp/";
        public const string DestinationBackendPath = DestinationRootPath + "Backend/";
        public const string DestinationBackendAppPath = DestinationBackendPath + "App/";
        public const string DestinationBackendBasePath = DestinationBackendPath + "Base/";
        
        public const string OriginCommonPath = OriginRootPath + "Common/";
        public const string OriginCommonBaseDtoPath = OriginCommonPath + "DTO/Base/";
        public const string OriginCommonAppDtoPath = OriginCommonPath + "DTO/App/";
        public const string OriginCommonBaseSearchPath = OriginCommonPath + "Search/Base/";
        public const string OriginCommonAppSearchPath = OriginCommonPath + "Search/App/";

        public const string EntityDirectory = "Ent";
        public const string ControllerPattern = "*Controller.cs";
        public const string ServicePattern = "*Service*";
        public const string RepositoryPattern = "*Repo*";
        public const string UlrsService = "*Repo*";

        public const string InsertCode = "//INSERT_GENERATED_CODE_HERE";
    }
}
