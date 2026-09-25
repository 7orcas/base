using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator.Util
{
    public class BaseGenerator
    {
        public BaseTemplate template;

        public BaseGenerator(BaseTemplate _template)
        {
            this.template = _template;
        }


        public async Task CreateFolder(string path, bool replace = false)
        {
            if (replace && Directory.Exists(path))
            {
                Log("Deleting directory: " + path);
                Directory.Delete(path, recursive: true);
            }

            if (!Directory.Exists(path))
            {
                Log("Creating directory: " + path);
                Directory.CreateDirectory(path);
            }

            //var entPath = Path.Combine(path, "Ent");
            //if (template.IsBackendEnt && !Directory.Exists(entPath))
            //{
            //    Log("Creating directory: " + entPath);
            //    Directory.CreateDirectory(entPath);
            //}

        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
