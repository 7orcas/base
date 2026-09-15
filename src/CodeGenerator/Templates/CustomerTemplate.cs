using System;
using System.Collections.Generic;
using System.Text;
using GC = CodeGenerator.GlobalConstants;

namespace CodeGenerator.Templates
{
    public class CustomerTemplate : BaseTemplate
    {
        private const string FromEntity = "Machine";

        public CustomerTemplate()
        {
            ToEntity = "Customer";
        }


        public override BaseTemplate ConfigureTemplate()
        {
            AppSubPath = "DS/";
            Backend = new ProjectTemplate
            {
                IsCreate = true,
                NewPath = GC.BackendAppPath + AppSubPath + ToEntity,
                CopyFromPath = GC.BackendAppPath + FromEntity,
                CopySubFolders = true,
                CopyControllers = true,
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
