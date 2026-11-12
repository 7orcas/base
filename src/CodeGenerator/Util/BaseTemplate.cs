using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeGenerator.Util
{
    public abstract class BaseTemplate
    {
        public string ToEntity { get; set; }
        public string AppSubPath { get; set; } = "";

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
        public bool IsNewReplace { get; set; } = false;
        public bool CopyEnts { get; set; } = false;
        public bool CopyControllers { get; set; } = false;
        public bool CopyServices { get; set; } = false;
        public bool CopyRepos { get; set; } = false;
        public List<StringChange> StringsToChange = new List<StringChange>();
        public List<StringRemove> StringsToRemove = new List<StringRemove>();

        // public string PermissionAtt = GC.PermissionAttribute;


        public bool IsValid()
        {
            return !string.IsNullOrEmpty(NewPath);
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

}
