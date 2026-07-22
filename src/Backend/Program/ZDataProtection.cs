using Microsoft.AspNetCore.DataProtection;
using GC = Backend.GlobalConstants;

namespace Backend.Program
{
    /// <summary>
    /// Data Protection is used by ASP.NET Core to securely:
    /// Encrypt and decrypt data
    /// - Protect authentication cookies
    /// - Protect anti - forgery tokens(CSRF tokens)
    /// - Protect session state data
    /// - Protect sensitive values stored outside the database
    /// </summary>
    public class ZDataProtection
    {
        static public void Configure(WebApplicationBuilder builder)
        {
            /*  This must change when deploying to Azure */
            builder.Services.AddDataProtection().SetApplicationName(GC.AppName);
            /* For Azure Blob Storage key storage, add the following NuGet package:
               Microsoft.AspNetCore.DataProtection.AzureStorage
            using Azure.Identity;
            builder.Services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(
                    new Uri("https://<storage>.blob.core.windows.net/dpkeys/keys.xml"),
                    new DefaultAzureCredential())
                .SetApplicationName(GC.AppName);
            */

        }
    }
}
