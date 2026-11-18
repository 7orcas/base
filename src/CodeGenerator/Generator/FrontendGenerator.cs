using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using GC = CodeGenerator.GlobalConstants;

namespace CodeGenerator.Generator
{
    public class FrontendGenerator : BaseGenerator, GeneratorI
    {
        private ProjectTemplate project;
        
        public FrontendGenerator(BaseTemplate template, ILogger logger)
            : base(template, logger)
        {
            project = template.Frontend;
        }

        public async Task Generate()
        {
            if (!project.IsValid() || !project.IsCreate) return;

            await CreateFolder(project.NewPath, replace: project.IsReplace);

            if (project.CopyRazors)
                await CopyFolder(project.CopyFromPath, project.NewPath, project);

            if (project.UpdateGlobalUsings != null)
                await UpdateGlobalUsings(project.UpdateGlobalUsings);

            if (project.UpdateAppRoutes != null)
                await UpdateAppRoutes(project.UpdateAppRoutes);

            if (project.UpdateMainMenu != null)
                await UpdateMainMenu(project.UpdateMainMenu);

            return;
        }
    }           
}
