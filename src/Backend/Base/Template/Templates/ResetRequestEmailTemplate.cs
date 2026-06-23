using Org.BouncyCastle.Asn1.Ocsp;

namespace Backend.Base.Template.Templates
{
	public class ResetRequestEmailTemplate
	{
		public Dictionary<string, object> Data { get; set; }

		public ResetRequestEmailTemplate(LoginEnt login)
		{
			Data = new Dictionary<string, object>
				{
					{ "AppName", "Blue" + login.Userid },
					{ "ResetLink", "resetrequest" }
				};
		}


		public string Template()
		{
			return @"Click the link below to choose a new password:
                    {{ResetLink}}

                    For your security, this link will expire in {{ExpiryTime}}.

                    If the link doesn’t work, you can copy and paste this token into the reset page:
                    Token: {{Token}}

                    If you didn’t request the support, please contact your IT help desk immediately.

                    Thanks,  
                    {{AppName}} Team";
		}

		public string TemplateHtml()
		{
			return Template();
		}

	}
}
