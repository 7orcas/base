using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using GC = CodeGenerator.GlobalConstants;

namespace CodeGenerator.Templates
{
    public class TrailTemplate : BaseTemplate
    {
   
        private string[] Changes = {
            "GC.PerCodeTemplate", "GC.PerTrail",
            "namespace Backend.CodeTemplate.Generic", "namespace Backend.App.HF.Trail",
            "base.zcodetemplate", "hf.trail",
            "entity.ToTable(\"zcodetemplate\", \"base\")", "entity.ToTable(\"trail\", \"hf\")",
            "CodeTemplateTitle", "Trails",
            "CodeTemplateSub", "TrailSb",
            "CodeTemplates", "Trails",
            "CodeTemplate", "Trail",
            "codeTemplate", "trail",
            "codetemplates", "trails",
            "codetemplate", "trail",
        };

        private string[] Remove = {  };

        public TrailTemplate()
        {
            Application = GC.BackendAppHf;
            LangKey = "Trail";
            FromEntity = "CodeTemplate";
            FromPermission = "PerCodeTemplate";
            ToEntity = "Trail";
            ToPermission = "PerTrail";
            ToPermissionValue = 20001;
            Route = "trails";
            PageCode = "tra001";
        }

        public override BaseTemplate ConfigureTemplate()
        {
            ConfigureCommon();
            ConfigureBackend();
            ConfigureFrontend();

            return this;
        }

        private void ConfigureCommon()
        {
            Common = new ProjectTemplate
            {
                IsReplace = false,
                IsCreate = true,
                
                CopyDto = new CopyFile
                {
                    FromFile = ProjectTemplate.CombinePath(GC.OriginCommonBaseDtoPath, $"{FromEntity}Dto.cs"),
                    ToFile = ProjectTemplate.CombinePath(GC.OriginCommonAppDtoPath, $"{ToEntity}Dto.cs")
                },

                CopySearch = new CopyFile
                {
                    FromFile = ProjectTemplate.CombinePath(GC.OriginCommonBaseSearchPath, $"{FromEntity}Search.cs"),
                    ToFile = ProjectTemplate.CombinePath(GC.OriginCommonAppSearchPath, $"{ToEntity}Search.cs")
                },

               
            };

            Common.PopulateChanges(Changes);
            Common.PopulateRemovals(Remove);

            Common.StringsToChange.Add(new StringChange
            {
                OldString = "*Created*",
                NewString = DateTime.Now.ToString("MMM yyyy")
            });
        }

        private void ConfigureBackend()
        {
            Backend = new ProjectTemplate
            {
                IsReplace = false,
                IsCreate = true,
                CopyFromPath = ProjectTemplate.CombinePath(GC.OriginBackendPath, FromEntity, "Generic"),
                CopyEnts = true,

                CopyControllers = true,
                CopyServices = true,
                CopyRepos = true,

                UpdateGlobalUsings = new ZUpdate
                {
                    File = ProjectTemplate.CombinePath(GC.OriginBackendPath, "GlobalUsings.cs"),
                    ToEntity = $"global using Backend.App.{Application}.{ToEntity};",
                    ToEntityEnt = $"global using Backend.App.{Application}.{ToEntity}.Ent;"
                },

                UpdateDbContext = new ZUpdate
                {
                    File = ProjectTemplate.CombinePath(GC.OriginBackendPath, "Data", "AppDbContext.cs"),
                    ToEntity = ToEntity
                },

                UpdateZService = new ZUpdate
                {
                    File = ProjectTemplate.CombinePath(GC.OriginBackendPath, "Program/App", Application, "ZServicesApp.cs"),
                    ToEntity = ToEntity
                },

                UpdateZRepo = new ZUpdate
                {
                    File = ProjectTemplate.CombinePath(GC.OriginBackendPath, "Program/App", Application, "ZReposApp.cs"),
                    ToEntity = ToEntity
                },

                NewPath = ProjectTemplate.CombinePath(GC.DestinationBackendAppPath, Application, ToEntity),
                UpdateCommonGlobalConstants = new PermissionConstant
                {
                    PlaceHolderConstant = "//HF_PERMISSION_CONSTANT",
                    Name = ToPermission,
                    Value = ToPermissionValue,
                    PlaceHolderObject = "//HF_PERMISSION_OBJECT",
                    LangKey = LangKey,
                    IsAdmin = false
                },

                UpdateUrlsService = new UrlServiceUpdate
                {
                    ServiceFile = ProjectTemplate.CombinePath(GC.OriginBackendPath, "App", Application, "Base/Urls", "UrlsService.cs"),
                    PermissionTemplate = $"GC.{FromPermission}",
                    PermissionNew = $"GC.{ToPermission}",
                    FromEntity = FromEntity,
                    ToEntity = ToEntity,
                    IsList = true,
                    IsUpdate = true,
                    IsCreate = true
                },

            };

            Backend.PopulateChanges(Changes);
            Backend.PopulateRemovals(Remove);

            Backend.StringsToChange.Add(new StringChange
            {
                OldString = "*Created*",
                NewString = DateTime.Now.ToString("MMM yyyy")
            });
        }

        private void ConfigureFrontend()
        {
            Frontend = new ProjectTemplate
            {
                IsReplace = false,
                IsCreate = true,
                CopyFromPath = ProjectTemplate.CombinePath(GC.OriginFrontendPath, FromEntity, "Generic"),
                
                UpdateGlobalUsings = new ZUpdate
                {
                    File = ProjectTemplate.CombinePath(GC.OriginFrontendPath, "GlobalUsings.cs"),
                    ToEntity = $"global using FrontendServer.Application.{Application}.{ToEntity};",
                },
                               
                NewPath = ProjectTemplate.CombinePath(GC.DestinationFrontendAppPath, Application, ToEntity),
                CopyRazors = true,

                UpdateAppRoutes = new AppRoute
                {
                    PlaceHolderRoute = "//INSERT_GENERATED_ROUTE_HERE",
                    Route = $"public const string {ToEntity}Route = \"{Route}\";",
                    PlaceHolderPageCode = "//INSERT_GENERATED_PAGE_CODE_HERE",
                    PageCode = $"public const string {ToEntity}PageCode = \"{PageCode}\";",
                    PlaceHolderPageCodeRoute = "//INSERT_GENERATED_PAGE_CODE_ROUTE_HERE",
                    PageCodeRoute = $"{ToEntity}PageCode, {ToEntity}Route,  GC.{ToPermission},"
                },

            };

            Frontend.PopulateChanges(Changes);
            Frontend.PopulateRemovals(Remove);

            Frontend.StringsToChange.Add(new StringChange
            {
                OldString = "*Created*",
                NewString = DateTime.Now.ToString("MMM yyyy")
            });
        }


        public override bool IsValid()
        {
            return Backend != null && Backend.IsValid()
                && Common != null 
                && Frontend != null && Frontend.IsValid();
        }
    }


}
