using System;
using System.Collections.Generic;
using System.Text;
using GC = CodeGenerator.GlobalConstants;

namespace CodeGenerator.Templates
{
    public class TrailTemplate : BaseTemplate
    {
        private const string FromEntityApp = GC.BackendAppDs;
        private const string FromDir = "Machines";
        private const string FromEntity = "Machine";

        private string[] Changes = {
            "GC.PerMachine", "GC.PerTrail",
            "GetMachines", "GetTrails",
            "Machines", "Trail",
            "Machine", "Trail",
            "machine", "trail",
        };

        private string[] Remove = {
            "StationPairs",
        };

        public TrailTemplate()
        {
            ToEntity = "Trail";
        }

        public override BaseTemplate ConfigureTemplate()
        {
            ConfigureBackend();


            return this;
        }

        private void ConfigureBackend()
        {
            AppSubPath = GC.BackendAppHf + "/";
            Backend = new ProjectTemplate
            {
                IsNewReplace = false,
                IsCreate = true,
                CopyFromPath = ProjectTemplate.CombinePath(GC.OriginBackendAppPath, FromEntityApp + "/" + FromDir),
                CopyEnts = true,
                CopyControllers = true,
                CopyServices = true,
                CopyRepos = true,

                NewPath = ProjectTemplate.CombinePath(GC.DestinationBackendAppPath, AppSubPath + ToEntity),
            };

            for (int i = 0; i < Changes.Length; i += 2)
            {
                var oldString = Changes[i];
                var newString = Changes[i + 1];
                Backend.StringsToChange.Add(new StringChange
                {
                    OldString = oldString,
                    NewString = newString
                });
            }

            for (int i = 0; i < Remove.Length; i++)
            {
                var oldString = Remove[i];
                Backend.StringsToRemove.Add(new StringRemove
                {
                    OldString = oldString,
                });
            }
        }

        public override bool IsValid()
        {
            return Backend != null && Backend.IsValid();
        }
    }


}
