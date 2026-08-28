using System;

namespace FrontendServer.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PageCodeAttribute : Attribute
    {
        public string Code { get; }

        public PageCodeAttribute(string code)
        {
            Code = code;
        }
    }
}