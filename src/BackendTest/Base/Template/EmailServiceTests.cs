using Backend.Base.Email;
using Backend.Base.Template.Ent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Superpower.Model;
using System.IO;

namespace BackendTest.Base.Template
{
    [TestClass]
    [TestCategory("UnitServiceBase")]
    public class EmailServiceTests : BaseServiceTest
    {
        EmailService service;

        public EmailServiceTests() : base()
        {

            var config = new ConfigurationBuilder()
                .AddUserSecrets<EmailServiceTests>() // important
                .Build();

            var settings = config
                .GetSection("EmailSettings")
                .Get<EmailSettings>();

            service = new EmailService(BuildServiceProvider(), Options.Create(settings));
        }


        //[TestMethod]
        //public async Task GenerateDocument()
        //{
        //    var request = new ResetRequestEmail();
        //        request.Data = new Dictionary<string, object>
        //        {
        //            { "name", "John boy" },
        //            { "orderId", "ORD 123" },
        //            { "company", "MyCompany" }
        //        };

        //    var email = service.RenderTemplate(request);
        //   // Assert.IsTrue(email.StartsWith("Hello Johnno"));


        //    await service.SendEmailAsync("js@7orcas.com", "Test Email", email);

        //}


    }
}

