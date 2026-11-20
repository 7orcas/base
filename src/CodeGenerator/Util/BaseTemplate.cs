using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeGenerator.Util
{
    public abstract class BaseTemplate
    {
        public string Application { get; set; }
        public string FromEntity { get; set; }
        public string FromPermission { get; set; }
        public string ToEntity { get; set; }
        public string Route { get; set; }
        public string PageCode { get; set; }
        public string ToPermission { get; set; }
        public int ToPermissionValue { get; set; }
        public int EntityTypeValue { get; set; }
        public string LangKey { get; set; }
        public string MenuItem { get; set; }
        public string MenuItemLangKey { get; set; }

        public ProjectTemplate Backend;
        public ProjectTemplate Common;
        public ProjectTemplate Frontend;

        public abstract bool IsValid();
        public abstract BaseTemplate ConfigureTemplate();
    }

    public class ProjectTemplate
    {
        public bool IsCreate { get; set; } = false;
        public string CopyFromPath { get; set; } = "";
        public string NewPath { get; set; }
        public bool IsReplace { get; set; } = false;
        public bool CopyEnts { get; set; } = false;
        public bool CopyRazors { get; set; } = false;
        public CopyFile CopyDto { get; set; } = null;
        public CopyFile CopySearch { get; set; } = null;
        public bool CopyControllers { get; set; } = false;
        public bool CopyServices { get; set; } = false;
        public bool CopyRepos { get; set; } = false;
        public EntityTypeConstant UpdateEntityType { get; set; } = null;
        public ZUpdate UpdateDbContext { get; set; } = null;
        public ZUpdate UpdateGlobalUsings { get; set; } = null;
        public ZUpdate UpdateZService { get; set; } = null;
        public ZUpdate UpdateZRepo { get; set; } = null;
        public PermissionConstant UpdateCommonGlobalConstants { get; set; } = null;
        public UrlServiceUpdate UpdateUrlsService { get; set; } = null;
        public MainMenu UpdateMenuService { get; set; } = null;
        public AppRoute UpdateAppRoutes { get; set; } = null;
        public MainMenu UpdateMainMenu { get; set; } = null;
        public List<StringChange> StringsToChange = new List<StringChange>();
        public List<StringRemove> StringsToRemove = new List<StringRemove>();

        // public string PermissionAtt = GC.PermissionAttribute;

        public void PopulateChanges(string[] changes)
        {
            for (int i = 0; changes != null && i < changes.Length; i += 2)
            {
                var oldString = changes[i];
                var newString = changes[i + 1];
                StringsToChange.Add(new StringChange
                {
                    OldString = oldString,
                    NewString = newString
                });
            }
        }

        public void PopulateRemovals(string[] removals)
        {
            for (int i = 0; removals != null && i < removals.Length; i++)
            {
                StringsToRemove.Add(new StringRemove
                {
                    OldString = removals[i]
                });
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(NewPath);
        }

        public static string CombinePath(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return string.Empty;

            string combined = paths[0] ?? string.Empty;

            for (int i = 1; i < paths.Length; i++)
            {
                string next = paths[i] ?? string.Empty;

                if (string.IsNullOrEmpty(combined))
                {
                    combined = next;
                    continue;
                }

                if (string.IsNullOrEmpty(next))
                    continue;

                if (!combined.EndsWith("/") && !next.StartsWith("/"))
                {
                    combined = $"{combined}/{next}";
                }
                else if (combined.EndsWith("/") && next.StartsWith("/"))
                {
                    combined += next.Substring(1);
                }
                else
                {
                    combined += next;
                }
            }

            return combined;
        }

    }

    public class StringChange
    {
        public string OldString { get; set; }
        public string NewString { get; set; }
        public bool MatchCase { get; set; } = true;

        public string Change(string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(this.OldString))
                return input;

            var options = this.MatchCase
                ? RegexOptions.None
                : RegexOptions.IgnoreCase;

            return Regex.Replace(
                input,
                Regex.Escape(this.OldString),
                this.NewString,
                options);
        }
    }

    public class StringRemove
    {
        public string OldString { get; set; }
        public bool MatchCase { get; set; } = true;
        public bool RemoveLine { get; set; } = true;

        public string Remove(string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(this.OldString))
                return input;

            if (RemoveLine)
            {
                var comparison = MatchCase
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase;

                var lines = input.Split(Environment.NewLine);

                return string.Join(
                    Environment.NewLine,
                    lines.Where(line => !line.Contains(OldString, comparison)));
            }

            var options = this.MatchCase
                ? RegexOptions.None
                : RegexOptions.IgnoreCase;

            return Regex.Replace(
                input,
                Regex.Escape(this.OldString),
                "",
                options);
        }

    }

    public class EntityTypeConstant
    {
        public string PlaceHolderType { get; set; }
        public string PlaceHolderObject { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string LangKey { get; set; }
    }

    public class PermissionConstant
    {
        public string PlaceHolderConstant { get; set; }
        public string PlaceHolderObject { get; set; }
        public string Name { get; set; }
        public string LangKey { get; set; }
        public int Value { get; set; }
        public bool IsAdmin { get; set; } = false;
    }

    public class UrlServiceUpdate
    {
        public string ServiceFile { get; set; }
        public string PermissionTemplate { get; set; }
        public string PermissionNew { get; set; }
        public string FromEntity { get; set; }
        public string ToEntity { get; set; }
        public bool IsList { get; set; } = true;
        public bool IsUpdate { get; set; } = true;
        public bool IsCreate { get; set; } = true;
    }

    public class ZUpdate
    {
        public string File { get; set; }
        public string ToEntity { get; set; }
        public string ToEntityEnt { get; set; }
    }

    public class CopyFile
    {
        public string FromFile { get; set; }
        public string ToFile { get; set; }
    }

    public class AppRoute
    {
        public string PlaceHolderRoute { get; set; }
        public string Route { get; set; }
        public string PlaceHolderPageCode { get; set; }
        public string PageCode { get; set; }
        public string PlaceHolderPageCodeRoute { get; set; }
        public string PageCodeRoute { get; set; }
    }

    public class MainMenu
    {
        public string File { get; set; }
        public string MenuItem { get; set; }
        public string MenuItemLangKey { get; set; }
        public string Route { get; set; }
        public string Permission { get; set; }
        public string PlaceHolder { get; set; }
    }
}

