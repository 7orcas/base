using GC = Backend.GlobalConstants;

/// <summary>
/// Email generation methods
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
/// 
namespace Backend.Base.Template
{
    public class EmailService : BaseService, EmailServiceI
    {
        public EmailService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }



    }
}
