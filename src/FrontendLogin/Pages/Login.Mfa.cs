
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

                var client = HttpClientFactory.CreateClient(GC.HTTP_Client);
                loginRequest.MfaCode = mfaCode;

                var response = await client.PostAsJsonAsync(GC.URL_mfa_verify, loginRequest);

                if (!response.IsSuccessStatusCode)
                {
                    errorMessage = "Invalid MFA code";
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<_ResponseDto>(json);
                var login = JsonConvert.DeserializeObject<LoginSuccessDto>(responseDto.Result.ToString());
                successMessage = responseDto.SuccessMessage;
                tokenkey = login.TokenKey;
                mainUrl = login.MainUrl;

                if (!responseDto.Valid)
                {
                    errorMessage = responseDto.ErrorMessage;
                    return;
                }

                // ✅ NOW complete login
                if (navigateTo)
                {
                    NavigationManager.NavigateTo(mainUrl
                        + "#tk=" + tokenkey
                        + (string.IsNullOrEmpty(autoOpen) ? "" : "&open=" + autoOpen)
                        + ("&urlLogin=https://localhost:7289/" + UrlSuffix),
                        true);

                    return;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

    }
}
