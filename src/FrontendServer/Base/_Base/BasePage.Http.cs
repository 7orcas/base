using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;
using System.Text;
using GC = FrontendServer.GlobalConstants;


/// <summary>
/// Configuration methods for Http stuff
/// Created: June 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace FrontendServer.Base._Base
{
    public partial class BasePage
    {
        //Http Client
        public async Task<HttpClient> GetClient() => await HS.GetClient();

        public async Task<HttpClient> GetClientU() => await HS.GetClientUnauthorized();

        public async Task ValidateAccess()
        {
            var refreshUrl = "";

            try
            {
                var client = await GetClient();
                var response = await client.GetAsync(GC.URL_token_test);
                response.EnsureSuccessStatusCode();

                var r = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<_ResponseDto>(r);
                //var expiry = JsonConvert.DeserializeObject<Boolean>(dto.Result.ToString());
                var expiry = dto.Result?.ToString().ToLower() == "true";

                if (expiry)
                    refreshUrl = GC.URL_token_refresh_current;
            }
            catch
            {
                refreshUrl = GC.URL_token_refresh_expired;
            }

            if (!string.IsNullOrEmpty(refreshUrl))
            {
                var clientU = await GetClientU();
                var refreshTokenString = "";
                var r1 = await PS.GetAsync<string>(GC.RefreshTokenCacheKey);
                if (r1.Success)
                    refreshTokenString = r1.Value;

                var response = await clientU.GetAsync(refreshUrl + "/" + refreshTokenString);
                var r2 = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<_ResponseDto>(r2);
                var tokenDto = JsonConvert.DeserializeObject<LoginTokenDto>(dto.Result.ToString());
                await PS.SetAsync(GC.TokenCacheKey, tokenDto.AccessToken);
                await PS.SetAsync(GC.RefreshTokenCacheKey, tokenDto.RefreshToken);
            }

        }

        public async Task<T> GetAsync<T>(string url)
        {
            return await GetAsync<T>(url, false);
        }

        public async Task<T> GetAsync<T>(string url, bool surpressLoading)
        {
            _isLoading = !surpressLoading;
            loadStatus.IsLoading = !surpressLoading;
            await ValidateAccess();

            var client = await GetClient();
            var response = await client.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();
                var r = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<_ResponseDto>(r);
                _statusCode = dto.StatusCode;
                _isLoading = false;
                loadStatus.ResetLoading()
                          .StatusCode = dto.StatusCode;

                if (dto.Valid)
                    return JsonConvert.DeserializeObject<T>(dto.Result.ToString());

                _errorMessage = new MarkupString(dto.ErrorMessage);
                loadStatus.ErrorMessage = new MarkupString(dto.ErrorMessage);
            }
            catch
            {
                
                if (response != null && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _statusCode = StatusCodes.Status401Unauthorized;
                    _errorMessage = new MarkupString(LS.GetLabel("LEx"));
                    _isLoading = false;
                    _isError = true;
                    
                    loadStatus.ResetLoading()
                              .SetError()
                              .SetErrorMessage(new MarkupString(LS.GetLabel("LEx")))
                              .SetStatusCode(StatusCodes.Status401Unauthorized);

                    return default;
                }

                _statusCode = -1;
                loadStatus.SetStatusCode(-1);
                try
                {
                    var r = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(r))
                    {
                        _errorMessage = new MarkupString(r);
                        loadStatus.SetErrorMessage(new MarkupString(r));
                    }
                    //ToDo label
                    else
                    {
                        _errorMessage = new MarkupString("Opps, something went wrong");
                        loadStatus.SetErrorMessage(new MarkupString("Opps, something went wrong"));
                    }
                }
                catch (Exception ex)
                {
                    _errorMessage = new MarkupString($"Exception occurred: {ex.Message}");
                    loadStatus.SetErrorMessage(new MarkupString($"Exception occurred: {ex.Message}"));
                }
            }

            _isLoading = false;
            _isError = true;
            loadStatus.ResetLoading()
                      .SetError();

            return default;
        }

        protected async Task<HttpResponseMessage> PostAsync<T>(string url, _BaseDto dto) where T : _BaseDto
        {
            _isSaving = true;
            loadStatus.SetSaving();
            await ValidateAccess();
            var client = await GetClient();

            var json = System.Text.Json.JsonSerializer.Serialize((T)dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            _isSaving = false;
            loadStatus.ResetSaving();
            return response;
        }

        protected async Task<HttpResponseMessage> PostAsync<T, S>(string url, S search) where S : _BaseSearch
        {
            loadStatus.SetSearch();
            await ValidateAccess();
            var client = await GetClient();

            var json = System.Text.Json.JsonSerializer.Serialize(search);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            loadStatus.ResetSearch();
            return response;
        }

        protected async Task<_ResponseDto> PostAsync<T>(string url, IEnumerable<T> dtos) where T : _BaseDto
        {
            _isSaving = true;
            loadStatus.SetSaving();
            _isValidationError = false;

            foreach (var d in dtos)
                d.IsError = false;

            await ValidateAccess();
            var client = await GetClient();

            var json = System.Text.Json.JsonSerializer.Serialize(dtos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            var r = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<_ResponseDto>(r);

            if (dto.StatusCode == StatusCodes.Status422UnprocessableEntity)
            {
                _isValidationError = true;
                var list = JsonConvert.DeserializeObject<List<ValDto>>(dto.Result.ToString());
                foreach (var v in list)
                {
                    var rec = dtos.FirstOrDefault(r => r.Id == v.Id);
                    if (rec != null)
                        rec.IsError = true;
                }
                await OpenValidationDialog(list);
            }


            _isSaving = false;
            loadStatus.ResetSaving();
            return dto;
        }

        protected Task OpenValidationDialog(List<ValDto> vals)
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                Position = DialogPosition.TopCenter,
                MaxWidth = MaxWidth.Medium
            };
            var key = !string.IsNullOrEmpty(_entityLangKey) ? _entityLangKey : "Entity";

            var parameters = new DialogParameters
            {
                { "Errors", vals},
                { "EntityKey", key }
            };

            var title = GetLabel("ValE");
            return DialogService.ShowAsync<ValidationDialog>(title, parameters, options);
        }
    }
}
