using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using GC = CodeGenerator.GlobalConstants;

namespace CodeGenerator.Templates
{
    public class TrailGroupTemplate : BaseTemplate
    {
   
        private string[] Changes = {
            "GC.PerCodeTemplate", "GC.PerTrailGroup",
            "namespace Backend.CodeTemplate.Generic", "namespace Backend.App.HF.TrailGroup",
            "base.zcodetemplate", "hf.trailGroup",
            "entity.ToTable(\"zcodetemplate\", \"base\")", "entity.ToTable(\"trailgroup\", \"hf\")",
            "CodeTemplateTitle", "TrailsGrp",
            "CodeTemplateSub", "TrailGrpSb",
            "CodeTemplates", "TrailGroups",
            "CodeTemplate", "TrailGroup",
            "codeTemplate", "trailGroup",
            "codetemplates", "trailgroups",
            "codetemplate", "trailgroup",
            "CodeTempSr", "TrailGrpSr",
        };

        private string[] Remove = {  };

        public TrailGroupTemplate()
        {
            Application = GC.BackendAppHf;
            LangKey = "TrailGrp";
            FromEntity = "CodeTemplate";
            FromPermission = "PerCodeTemplate";
            ToEntity = "TrailGroup";
            ToPermission = "PerTrailGroup";
            ToPermissionValue = 20002;
            Route = "trailGroups";
            PageCode = "trg001";
            MenuItem = "TrailGroup";
            MenuItemLangKey = "TrailsGrp";
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
                    PlaceHolderConstant = GC.InsertCode + ":PERM_CONSTANT",
                    Name = ToPermission,
                    Value = ToPermissionValue,
                    PlaceHolderObject = GC.InsertCode + ":PERM_OBJECT",
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

                UpdateMenuService = new MainMenu
                {
                    File = ProjectTemplate.CombinePath(GC.OriginBackendPath, "App", Application, "Base/Menu", "MenuService.cs"),
                    MenuItem = MenuItem,
                    Permission = ToPermission,
                    PlaceHolder = GC.InsertCode,
                }

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

                UpdateMainMenu = new MainMenu
                {
                    File = ProjectTemplate.CombinePath(GC.OriginFrontendPath, "Shared/TopMenu", "MainMenu.razor"),
                    MenuItem = MenuItem,
                    MenuItemLangKey = MenuItemLangKey,
                    Route = $"{ToEntity}Route",
                    PlaceHolder = GC.InsertCode,
                }

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
