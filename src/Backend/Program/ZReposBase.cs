namespace Backend.Program
{
    public class ZReposBase
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<LoginRepoI, LoginRepo>();
            builder.Services.AddScoped<RoleRepoI, RoleRepo>();
            builder.Services.AddScoped<TokenRepoI, TokenRepo>();
        }
    }
}
