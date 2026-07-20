using Common.Response;
//using Newtonsoft.Json;
using System.Text.Json;

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

        public RobotService(IServiceProvider serviceProvider,
            HttpClient httpClient)
            : base(serviceProvider)
        {
            _httpClient = httpClient;
        }

       
        public async Task<(bool success, string message)> Verify(string token)
        {
            var secret = "6LfX7VstAAAAABCxC2J9HZzB7hZJF9VrIoKOKthK";

            var response =
                await _httpClient.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}",
                    null);

            if (!response.IsSuccessStatusCode)
                return (false, "Failed to verify reCAPTCHA");

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<RobotResponse>(content);

            return (result?.Success == true, result?.Success == true ? "reCAPTCHA verified successfully" : "Failed to verify reCAPTCHA");
        }

       

    }
}
