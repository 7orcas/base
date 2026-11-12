using System;
using System.Collections.Generic;
using System.Text;
using GC = CodeGenerator.GlobalConstants;

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

            if (project.CopyEnts)
            {
                var dir = await CreateFolder(CombinePath(project.NewPath, GC.EntityDirectory), replace: project.IsNewReplace);
                await CopyFolder(CombinePath(project.CopyFromPath, GC.EntityDirectory), dir, project);
            }

            if (project.CopyControllers)
                await CopyFolder(project.CopyFromPath, project.NewPath, project, GC.ControllerPattern);

            if (project.CopyServices)
                await CopyFolder(project.CopyFromPath, project.NewPath, project, GC.ServicePattern);

            if (project.CopyRepos)
                await CopyFolder(project.CopyFromPath, project.NewPath, project, GC.RepositoryPattern);

            return;
        }
    }           
}
