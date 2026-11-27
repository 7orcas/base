
namespace Backend.Program.App.HF
{
    public class ZReposApp
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<CodeTemplateRepoI, CodeTemplateRepo>();

            builder.Services.AddScoped<TrialRepoI, TrialRepo>();
            builder.Services.AddScoped<TrialGroupRepoI, TrialGroupRepo>();
            //INSERT_GENERATED_CODE_HERE
        }
    }
}
