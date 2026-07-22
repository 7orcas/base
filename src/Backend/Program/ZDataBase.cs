using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Program
{
    public class ZDataBase
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(
                        builder.Configuration.GetConnectionString("DBMainConnection")));

        }
    }
}
