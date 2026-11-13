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
            "base.zcodetemplate", "hf.trail"
            "entity.ToTable(\"zcodetemplate\", \"base\")", "entity.ToTable(\"trail\", \"hf\")"
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
            FromEntity = "CodeTemplate";
            ToEntity = "Trail";
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
                CopyFromPath = ProjectTemplate.CombinePath(GC.OriginBackendPath, "Generic", FromEntity),
                CopyEnts = true,
                CopyControllers = true,
                CopyServices = true,
                CopyRepos = true,

                NewPath = ProjectTemplate.CombinePath(GC.DestinationBackendAppPath, GC.BackendAppHf, ToEntity),
                UpdateCommonGlobalConstants = "PerTrail     = 10001;"
                UpdateUrlsService = ProjectTemplate.CombinePath(GC.OriginBackendAppPath, GC.BackendAppHf, "Base/Urls"),

                PopulateChanges(Changes),
                PopulateRemovals(Remove)
            };

            Backend.StringsToChange.Add(new StringChange
            {
                OldString = "*Created*",
                NewString = ""
            });

            
        }

        public override bool IsValid()
        {
            return Backend != null && Backend.IsValid();
        }
    }


}
