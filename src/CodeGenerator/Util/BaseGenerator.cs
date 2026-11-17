using DocumentFormat.OpenXml.Presentation;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using GC = CodeGenerator.GlobalConstants;

namespace CodeGenerator.Util
{
    public class BaseGenerator
    {
        public BaseTemplate template;
        private readonly ILogger _logger;

        public BaseGenerator(BaseTemplate _template, ILogger logger)
        {
            this.template = _template;
            this._logger = logger;
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

                await CopyFile(sourceFile, destinationFile, project);
            }
        }

        public async Task CopyFile (string sourceFile, string destinationFile, ProjectTemplate project)
        {
            if (!project.IsReplace && File.Exists(destinationFile))
                return;

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

        public async Task UpdateCommonGlobalConstants(PermissionConstant permissionConstant)
        {
            try
            {
                var commonGCFile = Path.Combine(GC.OriginCommonPath, "GlobalConstants.cs");
                if (!File.Exists(commonGCFile))
                {
                    Log($"File not found: {commonGCFile}");
                    return;
                }

                var contents = await File.ReadAllTextAsync(commonGCFile);

                var permConstant = $"public const int {permissionConstant.Name}";
                var addPermConstant = !contents.Contains(permConstant) && contents.Contains(permissionConstant.PlaceHolderConstant);
                permConstant += $" = {permissionConstant.Value};";

                var permObject = $"{permissionConstant.Name}, \"{permissionConstant.LangKey}\", { (permissionConstant.IsAdmin ? "true" : "false")},";
                var addPermObject = !contents.Contains(permObject);


                if (addPermConstant)
                    contents = InsertAfterPlaceholder(contents, permissionConstant.PlaceHolderConstant, permConstant);
                else
                    Log($"{permissionConstant.Name} constant not added to {commonGCFile}");


                if (addPermObject)
                    contents = InsertAfterPlaceholder(contents, permissionConstant.PlaceHolderObject, permObject);
                else
                    Log($"{permissionConstant.Name} object not added to {commonGCFile}");


                await File.WriteAllTextAsync(commonGCFile, contents);
            }
            catch (Exception ex)
            {
                Log($"Error updating GlobalConstants.cs: {ex.Message}");
            }
        }


        public async Task UpdateUrlsService(UrlServiceUpdate urlServiceUpdate)
        {
            try
            {
                var urlsFile = urlServiceUpdate.ServiceFile;
                if (!File.Exists(urlsFile))
                {
                    Log($"File not found: {urlsFile}");
                    return;
                }

                var contents = await File.ReadAllTextAsync(urlsFile);

                var index = contents.IndexOf(GC.InsertCode);
                if (index == -1)
                {
                    Log($"Placeholder '{GC.InsertCode}' not found.");
                    return;
                }

                if (urlServiceUpdate.IsList)
                {
                    contents = ReplaceWithinBlockIfNotExists(
                        contents,
                        $"if (session.IsUserPermission({urlServiceUpdate.PermissionTemplate}))",
                        urlServiceUpdate);
                }

                if (urlServiceUpdate.IsUpdate)
                {
                    contents = ReplaceWithinBlockIfNotExists(
                        contents,
                        $"if (session.IsUserPermissionUpdate({urlServiceUpdate.PermissionTemplate}))",
                        urlServiceUpdate);
                }

                if (urlServiceUpdate.IsCreate)
                {
                    contents = ReplaceWithinBlockIfNotExists(
                        contents,
                        $"if (session.IsUserPermissionCreate({urlServiceUpdate.PermissionTemplate}))",
                        urlServiceUpdate);
                }


                await File.WriteAllTextAsync(urlsFile, contents);
            }
            catch (Exception ex)
            {
                Log($"Error updating UpdateUrlsService.cs: {ex.Message}");
            }
        }

        public async Task UpdateGlobalUsings(ZUpdate zUpdate)
        {
            try
            {
                var file = zUpdate.File;
                if (!File.Exists(file))
                {
                    Log($"File not found: {file}");
                    return;
                }

                var contents = await File.ReadAllTextAsync(file);

                var index = contents.IndexOf(GC.InsertCode);
                if (index == -1)
                {
                    Log($"Placeholder '{GC.InsertCode}' not found in {file}");
                    return;
                }

                var statement = zUpdate.ToEntity;
                index = contents.IndexOf(statement);
                if (index >= 0)
                {
                    Log($"{statement}' already exists in {file}");
                    return;
                }
                contents = InsertGeneratedCode(contents, statement);

                if (zUpdate.ToEntityEnt != null)
                {
                    statement = zUpdate.ToEntityEnt;
                    index = contents.IndexOf(statement);
                    if (index >= 0)
                    {
                        Log($"{statement}' already exists in {file}");
                        return;
                    }
                    contents = InsertGeneratedCode(contents, statement);
                }

                await File.WriteAllTextAsync(file, contents);
            }
            catch (Exception ex)
            {
                Log($"Error updating UpdateZService: {ex.Message}");
            }
        }

        public async Task UpdateDbContext(ZUpdate zUpdate)
        {
            try
            {
                var file = zUpdate.File;
                if (!File.Exists(file))
                {
                    Log($"File not found: {file}");
                    return;
                }

                var contents = await File.ReadAllTextAsync(file);

                var index = contents.IndexOf(GC.InsertCode);
                if (index == -1)
                {
                    Log($"Placeholder '{GC.InsertCode}' not found in {file}");
                    return;
                }

                var dbSet = $" public DbSet<{zUpdate.ToEntity}Ent> {zUpdate.ToEntity}s => Set<{zUpdate.ToEntity}Ent>();";
                index = contents.IndexOf(dbSet);
                if (index >= 0)
                {
                    Log($"{dbSet}' already exists in {file}");
                    return;
                }

                contents = InsertGeneratedCode(contents, dbSet);

                await File.WriteAllTextAsync(file, contents);
            }
            catch (Exception ex)
            {
                Log($"Error updating UpdateDbContext: {ex.Message}");
            }
        }

        public async Task UpdateZService(ZUpdate zUpdate)
        {
            try
            {
                var file = zUpdate.File;
                if (!File.Exists(file))
                {
                    Log($"File not found: {file}");
                    return;
                }

                var contents = await File.ReadAllTextAsync(file);

                var index = contents.IndexOf(GC.InsertCode);
                if (index == -1)
                {
                    Log($"Placeholder '{GC.InsertCode}' not found in {file}");
                    return;
                }

                var service = $"builder.Services.AddScoped<{zUpdate.ToEntity}ServiceI, {zUpdate.ToEntity}Service>();";
                index = contents.IndexOf(service);
                if (index >= 0)
                {
                    Log($"{service}' already exists in {file}");
                    return;
                }

                contents = InsertGeneratedCode(contents, service);

                await File.WriteAllTextAsync(file, contents);
            }
            catch (Exception ex)
            {
                Log($"Error updating UpdateZService: {ex.Message}");
            }
        }

        public async Task UpdateZRepo(ZUpdate zUpdate)
        {
            try
            {
                var file = zUpdate.File;
                if (!File.Exists(file))
                {
                    Log($"File not found: {file}");
                    return;
                }

                var contents = await File.ReadAllTextAsync(file);

                var index = contents.IndexOf(GC.InsertCode);
                if (index == -1)
                {
                    Log($"Placeholder '{GC.InsertCode}' not found in {file}");
                    return;
                }

                var service = $"builder.Services.AddScoped<{zUpdate.ToEntity}RepoI, {zUpdate.ToEntity}Repo>();";
                index = contents.IndexOf(service);
                if (index >= 0)
                {
                    Log($"{service}' already exists in {file}");
                    return;
                }

                contents = InsertGeneratedCode(contents, service);

                await File.WriteAllTextAsync(file, contents);
            }
            catch (Exception ex)
            {
                Log($"Error updating UpdateZRepo: {ex.Message}");
            }
        }

        public async Task UpdateAppRoutes(AppRoute appRoute)
        {
            try
            {
                var appRoutesFile = Path.Combine(GC.OriginFrontendAppPath, "AppRoutes.cs");
                if (!File.Exists(appRoutesFile))
                {
                    Log($"File not found: {appRoutesFile}");
                    return;
                }

                var contents = await File.ReadAllTextAsync(appRoutesFile);

                var addRoute = !contents.Contains(appRoute.Route) && contents.Contains(appRoute.PlaceHolderRoute);
                var addPageCode = !contents.Contains(appRoute.PageCode) && contents.Contains(appRoute.PlaceHolderPageCode);
                var addPageCodeRoute = !contents.Contains(appRoute.PageCodeRoute) && contents.Contains(appRoute.PlaceHolderPageCodeRoute);

                if (addRoute)
                    contents = InsertBeforePlaceholder(contents, appRoute.PlaceHolderRoute, appRoute.Route);
                else
                    Log($"{appRoute.Route} not added to {appRoutesFile}");


                if (addPageCode)
                    contents = InsertBeforePlaceholder(contents, appRoute.PlaceHolderPageCode, appRoute.PageCode);
                else
                    Log($"{appRoute.PageCode} not added to {appRoutesFile}");

                if (addPageCodeRoute)
                    contents = InsertBeforePlaceholder(contents, appRoute.PlaceHolderPageCodeRoute, appRoute.PageCodeRoute);
                else
                    Log($"{appRoute.PageCodeRoute} not added to {appRoutesFile}");


                await File.WriteAllTextAsync(appRoutesFile, contents);
            }
            catch (Exception ex)
            {
                Log($"Error updating GlobalConstants.cs: {ex.Message}");
            }
        }


        public string InsertGeneratedCode(
                string contents,
                string code)
        {
            var markerIndex = contents.IndexOf(GC.InsertCode);

            if (markerIndex < 0)
                return contents;

            var indent = GetIndent(contents, GC.InsertCode);
            contents = contents.Insert(markerIndex, code + Environment.NewLine + indent);

            return contents;
        }


        public string ReplaceWithinBlockIfNotExists(
                string contents,
                string marker,
                UrlServiceUpdate urlServiceUpdate)
        {
            var markerIndex = contents.IndexOf(marker);

            if (markerIndex < 0)
                return contents;

            var indent = GetIndent(contents, "if");
            var openBraceIndex = contents.IndexOf('{', markerIndex);

            if (openBraceIndex < 0)
                return contents;

            int depth = 1;
            int closeBraceIndex = openBraceIndex + 1;

            while (closeBraceIndex < contents.Length && depth > 0)
            {
                if (contents[closeBraceIndex] == '{')
                    depth++;
                else if (contents[closeBraceIndex] == '}')
                    depth--;

                closeBraceIndex++;
            }

            if (depth > 0)
                throw new InvalidOperationException("Matching closing brace not found.");

            var blockLength = closeBraceIndex - markerIndex;
            var block = contents.Substring(markerIndex, blockLength);

            var updatedBlock = block.Replace(urlServiceUpdate.PermissionTemplate, urlServiceUpdate.PermissionNew);
            updatedBlock = updatedBlock.Replace(urlServiceUpdate.FromEntity, urlServiceUpdate.ToEntity);

            if (contents.Contains(updatedBlock))
            {
                Log($"Updated block already exists in the file.");
                return contents;
            }

            updatedBlock += Environment.NewLine + Environment.NewLine + indent;

            var endIndex = contents.IndexOf(GC.InsertCode);

            contents = contents.Insert(endIndex, updatedBlock);

            return contents;
        }


        public string InsertBeforePlaceholder(string contents, string placeholder, string newContent)
        {
            var index = contents.IndexOf(placeholder);
            if (index == -1)
            {
                Log($"Placeholder '{placeholder}' not found.");
                return contents;
            }

            var indent = GetIndent(contents, placeholder);
            var newLine = $"{newContent}{Environment.NewLine}{indent}";

            return contents.Insert(index, newLine);
        }

        public string InsertAfterPlaceholder(string contents, string placeholder, string newContent)
        {
            var index = contents.IndexOf(placeholder);
            if (index == -1)
            {
                Log($"Placeholder '{placeholder}' not found.");
                return contents;
            }
            var indent = GetIndent(contents, placeholder);
            var newLine = $"{Environment.NewLine}{indent}{newContent}";

            return contents.Insert(index + placeholder.Length, newLine);
        }

        public string GetIndent(string contents, string placeholder)
        {
            var match = Regex.Match(
                contents,
                $@"^(?<indent>\s*){Regex.Escape(placeholder)}",
                RegexOptions.Multiline);

            var indent = match.Success
                ? match.Groups["indent"].Value
                       .Replace("\r", "")
                       .Replace("\n", "")
                : string.Empty;

            return indent;
        }


        public void Log(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
