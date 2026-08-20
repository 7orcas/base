using System;

namespace FrontendServer.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PagePermissionAttribute : Attribute
    {
        public int Code { get; }

        public PagePermissionAttribute(int code)
        {
            Code = code;
        }
    }
}