using Newtonsoft.Json;
using QRCoder;
using System.Data;
using GC = FrontendLogin.GlobalConstants;

/// <summary>
/// Self Registration methods for Login
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendLogin.Pages
{
    public partial class Login
    {
        private async Task SubmitSignup()
        {
            signupMessage = "";

            if (signupRequest.Password != signupRequest.ConfirmPassword)
            {
                signupMessage = GetLabel("PWx"); 
                return;
            }

            isProcessing = true;

            if (options.SelfRegistrationCaptcha)
            {
                var robotClient = HttpClientFactory.CreateClient(GC.HTTP_Client);
                var token = await _captcha.GetToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    signupMessage = GetLabel("CaptchaR");
                    return;
                }
                var robotResponse = await robotClient.PostAsJsonAsync(
                    GC.URL_robot,
                    new RobotRequest
                    {
                        LangCode = options.LangCode,
                        AppClient = GC.AppClient,
                        CaptchaToken = token
                    });

                if (!robotResponse.IsSuccessStatusCode)
                {
                    signupMessage = GetLabel("CaptchaE");
                    isProcessing = false;
                    return;
                }
                else
                {
                    var result = await robotResponse.Content.ReadAsStringAsync();
                    var responseDto = JsonConvert.DeserializeObject<_ResponseDto>(result);
                    var robot = JsonConvert.DeserializeObject<RobotDto>(responseDto.Result.ToString());

                    signupRequest.NotRobot = responseDto.Valid;
                    signupRequest.Token = robot.Token;
                    signupMessage = robot.Message;

                    if (!responseDto.Valid)
                    {
                        isProcessing = false;
                        return;
                    }
                }
            }
            else
            {
                signupRequest.NotRobot = false;
                signupRequest.Token = string.Empty;
            }

            var client = HttpClientFactory.CreateClient(GC.HTTP_Client);

            var response = await client.PostAsJsonAsync(
                GC.URL_signup,
                signupRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<_ResponseDto>(result);

                if (responseDto.Valid)
                {
                    showSignUpModal = false;
                    showSignUpSuccess = true;
                    initialiseSignupRequest();
                }
                signupMessage = responseDto.Result?.ToString();
            }
            else
            {
                signupMessage = GetLabel("SignUpFail") + "<br>" + GetLabel("SysA");
            }

            isProcessing = false;
        }

        private void initialiseSignupRequest()
        {
            signupRequest = new();
            signupRequest.OrgNr = options.OrgNr;
            signupRequest.LangCode = options.LangCode;
            signupRequest.Token = string.Empty;
            signupRequest.NotRobot = false;
            showSignupPassword = false;
        }

        private void GoToSelfRegistration()
        {
            signupMessage = "";
            showSignUpModal = true;
        }


        private void CloseSignupModal()
        {
            showSignUpModal = false;
            showSignUpSuccess = false;
        }

        private string GetPasswordRulesLabel()
        {
            if (showRules) return LS.GetLabel("PWhr", labels, isDev);
            return LS.GetLabel("PWsr", labels, isDev);
        }

        private async void ShowRules()
        {
            showRules = !showRules;
            rules = string.Empty;

            if (showRules)
            {
                var url = GC.URL_show_password_rules
                                        + "?orgNr=" + options.OrgNr
                                        + "&langcode=" + options.LangCode;

                var client = HttpClientFactory.CreateClient(GC.HTTP_Client);
                var response = await client.GetAsync(url);
                var r = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<_ResponseDto>(r);

                rules = dto.Result.ToString();
            }
            StateHasChanged();
        }

        private string passwordInputType => showPassword ? "text" : "password";

        public class RobotModel
        {
            public string Name { get; set; }
        }

    }
}
