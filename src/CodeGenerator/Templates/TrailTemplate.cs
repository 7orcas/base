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
        }

        public override BaseTemplate ConfigureTemplate()
        {
            ConfigureBackend();


            return this;
        }

        private void ConfigureBackend()
        {
            Backend = new ProjectTemplate
            {
                IsNewReplace = true,
                IsCreate = true,
                CopyFromPath = ProjectTemplate.CombinePath(GC.OriginBackendPath, FromEntity, "Generic"),
                CopyEnts = true,
                CopyControllers = true,
                CopyServices = true,
                CopyRepos = true,

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

        public override bool IsValid()
        {
            return Backend != null && Backend.IsValid();
        }
    }


}
