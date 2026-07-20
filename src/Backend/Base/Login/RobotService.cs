using Backend.Base.Token.Ent;
using Serilog.Core;
using System.Text.Json;
using System.Text.Json.Serialization;
using GC = Backend.GlobalConstants;

/// <summary>
/// Manage Not A Robot process for user
/// Created: July 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
namespace Backend.Base.Login
{
    public class RobotService : BaseService, RobotServiceI
    {
        private readonly HttpClient _httpClient;
        private readonly LabelServiceI _labelService;
        private readonly TokenServiceI _tokenService;

        public RobotService(IServiceProvider serviceProvider,
            HttpClient httpClient,
            LabelServiceI labelService,
            TokenServiceI tokenService)
            : base(serviceProvider)
        {
            _httpClient = httpClient;
            _labelService = labelService;
            _tokenService = tokenService;
        }

       
        public async Task<(bool success, string temptoken, string message)> Verify(string ipAddress, string token, string langCode)
        {
            var secret = AppSettings.ReCaptcha.SecretKey; 
            var labels = await _labelService.GetLangCodeDic(langCode, GC.LangLabelVariantDefault);

            var response =
                await _httpClient.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}",
                    null);

            if (!response.IsSuccessStatusCode)
                return (false, 
                    null,
                    GetLabel("CaptchaE", "Failed to verify reCAPTCHA", labels));

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<RobotResponse>(content);


            _log.Debug("reCAPTCHA response: {Content}", content);

            var tv = new TokenValues
            {
                IpAddress = ipAddress,
                Username = "xxx",
                SessionKey = "NoSession",
                OrgNr = 0,
            };

            var tempToken = _tokenService.CreateCaptchaToken(tv);

            return (result?.Success == true,
                tempToken,
                result?.Success == true ? 
                GetLabel("CaptchaS", "reCAPTCHA verified successfully", labels) :
                GetLabel("CaptchaE", "Failed to verify reCAPTCHA", labels));
        }

        public class RobotResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("challenge_ts")]
            public string ChallengeTs { get; set; }

            [JsonPropertyName("hostname")]
            public string Hostname { get; set; }

            [JsonPropertyName("error-codes")]
            public string[] ErrorCodes { get; set; }
        }


    }
}
