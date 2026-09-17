using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Backend.Core.Middleware
{
    public abstract class AuditActionConstants
    {
        public const string AuditCapture = "AuditCapture";
        public const string AuditArg = "AuditActionArguments";
        public const string AuditBefore = "B";
        public const string AuditAfter = "A";

        public const string AuditC = "AuditCreates";
        public const string AuditU = "AuditUpdates";
        public const string AuditD = "AuditDeletes";

        public AuditListAtt? GetAuditListAtt(ControllerActionDescriptor controllerActionDescriptor)
        {
            if (controllerActionDescriptor != null)
            {
                //Method ignore
                MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;
                var ignore = methodInfo.GetCustomAttribute<AuditIgnoreAtt>();

                //If not ignore then get entity type
                if (ignore == null)
                    return null;

                var attr = new AuditListAtt();

                //Method assign attibutes (first priority)
                var audit = methodInfo.GetCustomAttribute<AuditListAtt>();
                if (audit != null)
                {
                    attr.EntityTypeNr = audit.EntityTypeNr;
                    attr.CrudAction = audit.CrudAction;
                }

                //Class assigned attibutes (second priority)
                var controllerType = controllerActionDescriptor.ControllerTypeInfo;
                var classAudit = controllerType.GetCustomAttribute<AuditListAtt>();
                if (classAudit != null)
                {
                    if (attr.EntityTypeNr < 1)
                        attr.EntityTypeNr = classAudit.EntityTypeNr;
                    if (attr.CrudAction == null)
                        attr.CrudAction = classAudit.CrudAction;
                }

                return attr;
            }

            return null;
        }

    }
}
