namespace Backend.Program.App.HF
{
    public class ZReposApp
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<CodeTemplateRepoI, CodeTemplateRepo>();

            //INSERT_GENERATED_CODE_HERE
        }
    }
}
