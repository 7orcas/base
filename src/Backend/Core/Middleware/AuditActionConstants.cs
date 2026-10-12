using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Backend.Core.Middleware
{
    public abstract class AuditActionConstants
    {
        public const string AuditCapture = "AuditCapture";

        public const string AuditCreate = "AuditCreates";
        public const string AuditUpdate = "AuditUpdates";
        public const string AuditDelete = "AuditDeletes";
        public const string AuditInfo = "AuditInfo";

        public AuditListAtt? GetAuditListAtt(ControllerActionDescriptor controllerActionDescriptor)
        {
            if (controllerActionDescriptor != null)
            {
                //Method ignore
                MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;
                var ignore = methodInfo.GetCustomAttribute<AuditIgnoreAtt>();

                //Ignore audits
                if (ignore != null)
                    return null;

                var attr = new AuditListAtt();

                //Method assign attibutes (first priority)
                var audit = methodInfo.GetCustomAttribute<AuditListAtt>();
                if (audit != null)
                {
                    attr.EntityTypeNr = audit.EntityTypeNr;
                    attr.Action = audit.Action;
                }

                //Class assigned attibutes (second priority)
                var controllerType = controllerActionDescriptor.ControllerTypeInfo;
                var classAudit = controllerType.GetCustomAttribute<AuditListAtt>();
                if (classAudit != null)
                {
                    if (attr.EntityTypeNr < 1)
                        attr.EntityTypeNr = classAudit.EntityTypeNr;
                    if (attr.Action == null)
                        attr.Action = classAudit.Action;
                }

                return attr;
            }

            return null;
        }

    }

    public class AuditInfo 
    { 
        public long? Id { get; set; }
        public string? Code { get; set; }
        public int? Version { get; set; }
        public string? Json { get; set; }
    }

}
