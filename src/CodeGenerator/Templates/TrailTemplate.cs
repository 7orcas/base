using System;
using System.Collections.Generic;
using System.Text;
using GC = CodeGenerator.GlobalConstants;

namespace CodeGenerator.Templates
{
    public class TrailTemplate : BaseTemplate
    {
        private const string FromEntityApp = GC.BackendAppDs;
        private const string FromEntity = "Machine";

        public TrailTemplate()
        {
            ToEntity = "Trail";
        }


        public override BaseTemplate ConfigureTemplate()
        {
            AppSubPath = GC.BackendAppHf + "/";
            Backend = new ProjectTemplate
            {
                IsCreate = true,
                NewPath = GC.BackendAppPath + AppSubPath + ToEntity,
                CopyFromPath = GC.BackendAppPath + FromEntityApp + "/" + FromEntity,
                CopyEnts = true,
                CopyControllers = true,
                CopyServices = true,
                CopyRepos = true,
            };

            
            return this;
        }

        public override bool IsValid()
        {
            return Backend != null && Backend.IsValid();
        }
    }


}
