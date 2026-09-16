using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator.Util
{
    public abstract class BaseTemplate
    {
        public string ToEntity { get; set; }
        public string AppSubPath { get; set; } = "";

        public ProjectTemplate Backend;
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
        public bool CopySubFolders { get; set; } = false;
        public bool CopyControllers { get; set; } = false;
        public bool CopyServices { get; set; } = false;
        public bool CopyRepos { get; set; } = false;
        public string PermissionAtt = GC.PermissionAttribute;


        public bool IsValid()
        {
            return !string.IsNullOrEmpty(NewPath);
        }
    }

}
