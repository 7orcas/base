using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator.Generator
{
    public class BackendGenerator : BaseGenerator, GeneratorI
    {
        private ProjectTemplate project;

        public BackendGenerator(BaseTemplate template)
            : base(template) 
        {
            project = template.Backend;
        }

        public async Task Generate()
        {
            if (!project.IsValid() || !project.IsCreate) return;

            await CreateFolder(project.NewPath, replace: project.IsNewReplace);

            return;
        }
    }           
}
