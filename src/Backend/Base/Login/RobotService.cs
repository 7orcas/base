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

        public RobotService(IServiceProvider serviceProvider,
            HttpClient httpClient,
            LabelServiceI labelService)
            : base(serviceProvider)
        {
            _httpClient = httpClient;
            _labelService = labelService;
        }

       
        public async Task<(bool success, string message)> Verify(string token, string langCode)
        {
            var secret = AppSettings.ReCaptcha.SecretKey; 

            var response =
                await _httpClient.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}",
                    null);

            if (!response.IsSuccessStatusCode)
                return (false, "Failed to verify reCAPTCHA");

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<RobotResponse>(content);

            var labels = await _labelService.GetLangCodeDic(langCode, GC.LangLabelVariantDefault);

            _log.Debug("reCAPTCHA response: {Content}", content);

            return (result?.Success == true, result?.Success == true ? 
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
