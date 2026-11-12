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


        public async Task<string> CreateFolder(string path, bool replace = false)
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

            return path;
        }


        public async Task CopyFolder(string sourcePath, string destinationPath, ProjectTemplate project)
        {
            await CopyFolder(sourcePath, destinationPath, project, "*");
        }

        public async Task CopyFolder(string sourcePath, string destinationPath, ProjectTemplate project, string searchPattern)
        {
            foreach (var sourceFile in Directory.GetFiles(sourcePath, searchPattern))
            {
                var fileName = Path.GetFileName(sourceFile);

                foreach (var stringChange in project.StringsToChange)
                {
                    fileName = stringChange.Change(fileName);
                }

                var destinationFile = Path.Combine(destinationPath, fileName);

                if (project.IsNewReplace || !File.Exists(destinationFile))
                    await CopyFile(sourceFile, destinationFile, project);
                
            }
        }

        public async Task CopyFile (string sourceFile, string destinationFile, ProjectTemplate project)
        {
            var contents = File.ReadAllText(sourceFile);
            
            foreach (var stringChange in project.StringsToChange)
                contents = stringChange.Change(contents);
            
            foreach (var stringRemove in project.StringsToRemove)
                contents = stringRemove.Remove(contents);
            
            // Write modified file
            File.WriteAllText(destinationFile, contents);
        }


        public static string CombinePath(string first, string second)
        {
            first ??= string.Empty;
            second ??= string.Empty;

            if (string.IsNullOrEmpty(first))
                return second;

            if (string.IsNullOrEmpty(second))
                return first;

            if (!first.EndsWith("/") && !second.StartsWith("/"))
                return $"{first}/{second}";

            if (first.EndsWith("/") && second.StartsWith("/"))
                return first + second[1..];

            return first + second;
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
