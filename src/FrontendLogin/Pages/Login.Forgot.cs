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

        private async Task SubmitForgot()
        {
            if(string.IsNullOrWhiteSpace(forgotEmail))
            {
                forgotModalMessage = GetLabel("PWResetErr4");
                return;
            }

            var client = HttpClientFactory.CreateClient(GC.HTTP_Client);
            var response = await client.GetAsync($"{GC.URL_reset_request}?email={forgotEmail}");
            var r = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<_ResponseDto>(r);

            if (dto.Valid)
                forgotModalMessage = GetLabel("PWReset1") + "<br>" + GetLabel("PWReset2") + "<br>" + GetLabel("PWReset3");
            else
                forgotModalMessage = GetLabel("Oops");

            showForgotModal = false;
            showForgotSent = true;
            await InvokeAsync(StateHasChanged);
        }


        private void CloseForgotModal()
        {
            showForgotModal = false;
            showForgotSent = false;
        }
    }
}
