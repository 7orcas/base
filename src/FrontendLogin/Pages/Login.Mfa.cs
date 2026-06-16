
using Newtonsoft.Json;
using QRCoder;
using GC = FrontendLogin.GlobalConstants;

/// <summary>
/// MFA methods for Login
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendLogin.Pages
{
    public partial class Login
    {
        private async Task MfaCheck(LoginSuccessDto login)
        {
            loginRequest.Id = login.Id;

            if (!login.MfaEnabled)
                await LoadMfaQr(); //Show the QR code to set up MFA
            else
            {
                showMfaInput = true;
                StateHasChanged();
            }
        }


        private async Task LoadMfaQr()
        {
            var client = HttpClientFactory.CreateClient(GC.HTTP_Client);

            var response = await client.PostAsJsonAsync(GC.URL_mfa_setup, loginRequest.Id);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<_ResponseDto>(result);

                if (responseDto.Valid)
                {
                    var mfa = JsonConvert.DeserializeObject<MfaSetupDto>(responseDto.Result.ToString());
                    qrCodeBase64 = GenerateQrCode(mfa.QrCodeUri);
                    manualKey = mfa.SharedKey;
                    showQr = true;
                    showMfaInput = true;
                    StateHasChanged();
                    return;
                }
            }

            errorMessage = "Failed to load MFA setup";
        }

        private string GenerateQrCode(string uri)
        {
            using var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            var png = new PngByteQRCode(data);
            var bytes = png.GetGraphic(20);

            return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
        }

        private async Task VerifyMfa()
        {
            try
            {
                errorMessage = "";
                loginRequest.MfaCode = mfaCode;

                var login = await LoginToBlue(GC.URL_mfa_verify);

                if (login == null)
                    return;

                //Web clients
                if (navigateToMain)
                {
                    NavigateToMain(login);
                    return;
                }

                //API clients just get the token
                await GetToken(login.TokenKey);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

    }
}
