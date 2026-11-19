namespace Backend.Program.App.HF
{
    public class ZReposApp
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<CodeTemplateRepoI, CodeTemplateRepo>();

            builder.Services.AddScoped<TrailRepoI, TrailRepo>();
            builder.Services.AddScoped<TrailGroupRepoI, TrailGroupRepo>();
            //INSERT_GENERATED_CODE_HERE
        }
    }
}
