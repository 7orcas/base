
using GC = Backend.GlobalConstants;

/// <summary>
/// Template generation methods
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
/// 
namespace Backend.Base.Template
{
    public class TemplateService : BaseService, TemplateServiceI
    {
        private readonly OrgServiceI _orgService;


        public TemplateService(IServiceProvider serviceProvider, 
            OrgServiceI orgService)
            : base(serviceProvider)
        {
            _orgService = orgService;
        }

        //public async Task<string?> GetResetRequestEmail(LoginEnt login)
        //{

        //}


        //public async Task<string?> GetResetRequestEmail(LoginEnt login)
        //{
        //    try
        //    {
        //        var org = await _orgService.GetOrg(login.OrgNrDefault);
        //        if (org == null || !org.IsActive)
        //        {
        //            return null;
        //        }

        //        var request = new ResetRequestEmailTemplate(login);
        //        var email = RenderTemplate(request);
        //        return email;
        //    }
        //    catch { 
        //        return null;
        //    }

        //}

        private string RenderTemplate(ResetRequestEmailTemplate request)
        {
            if (string.IsNullOrWhiteSpace(request.Template()))
                throw new ArgumentException("Template cannot be empty");

            var template = Scriban.Template.Parse(request.Template());

            if (template.HasErrors)
                throw new Exception("Template parsing failed");

            var result = template.Render(request.Data, member => member.Name);

            return result;
        }

    }
}
