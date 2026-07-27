using Newtonsoft.Json;
using GC = FrontendLogin.GlobalConstants;

/// <summary>
/// Forgot UserName and/or Password methods for Login
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendLogin.Pages
{
    public partial class Login
    {
        private void GoToForgot()
        {
            forgotEmail = "";
            forgotModalMessage = "";
            showForgotModal = true;
        }

        private void initialiseResetRequest()
        {
            resetRequest = new();
            resetRequest.LangCode = options.LangCode;
            resetRequest.Token = string.Empty;
            resetRequest.NotRobot = false;
        }

        private async Task SubmitForgot()
        {
            if(string.IsNullOrWhiteSpace(forgotEmail))
            {
                forgotModalMessage = GetLabel("PWResetErr4");
                return;
            }

            isProcessing = true;

            if (options.PasswordResetCaptcha)
            {
                var robotClient = HttpClientFactory.CreateClient(GC.HTTP_Client);
                var token = await _captcha.GetToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    forgotMessage = GetLabel("CaptchaR");
                    isProcessing = false;
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
                    forgotMessage = GetLabel("CaptchaE");
                    isProcessing = false;
                    return;
                }
                else
                {
                    var result = await robotResponse.Content.ReadAsStringAsync();
                    var responseDto = JsonConvert.DeserializeObject<_ResponseDto>(result);
                    var robot = JsonConvert.DeserializeObject<RobotDto>(responseDto.Result.ToString());

                    resetRequest.NotRobot = responseDto.Valid;
                    resetRequest.Token = robot.Token;
                    forgotMessage = robot.Message;

                    if (!responseDto.Valid)
                    {
                        isProcessing = false;
                        return;
                    }
                }
            }
            else
            {
                resetRequest.NotRobot = false;
                resetRequest.Token = string.Empty;
            }

            resetRequest.Email = forgotEmail;
            resetRequest.LangCode = options.LangCode;

            var client = HttpClientFactory.CreateClient(GC.HTTP_Client);

            var response = await client.PostAsJsonAsync(
                GC.URL_reset_request,
                resetRequest);

            var r = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<_ResponseDto>(r);

            if (dto.Valid)
                forgotModalMessage = GetLabel("PWReset1") + "<br>" + GetLabel("PWReset2") + "<br>" + GetLabel("PWReset3");
            else
                forgotModalMessage = GetLabel("Oops");

            showForgotModal = false;
            showForgotSent = true;
            isProcessing = false;
            await InvokeAsync(StateHasChanged);
        }


        private void CloseForgotModal()
        {
            showForgotModal = false;
            showForgotSent = false;
        }
    }
}
