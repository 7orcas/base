using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using GC = CodeGenerator.GlobalConstants;

namespace CodeGenerator.Generator
{
    public class CommonGenerator : BaseGenerator, GeneratorI
    {
        private ProjectTemplate project;
        
        public CommonGenerator(BaseTemplate template, ILogger logger)
            : base(template, logger)
        {
            project = template.Common;
        }

        public async Task Generate()
        {
            if (!project.IsCreate) return;

            
            if (project.CopyDto != null)
                await CopyFile(project.CopyDto.FromFile, project.CopyDto.ToFile, project);
            
            if (project.CopySearch != null)
                await CopyFile(project.CopySearch.FromFile, project.CopySearch.ToFile, project);
            
            
            return;
        }
    }           
}
