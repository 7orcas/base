using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
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

        public async Task<T> GetAsync<T>(string url)
        {
            return await GetAsync<T>(url, false);
        }

        public async Task<T> GetAsync<T>(string url, bool surpressLoading)
        {
            _isLoading = !surpressLoading;

            var client = await GetClient();

            var response = await client.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();
                var r = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<_ResponseDto>(r);
                _statusCode = dto.StatusCode;
                _isLoading = false;

                if (dto.Valid)
                    return JsonConvert.DeserializeObject<T>(dto.Result.ToString());

                _errorMessage = new MarkupString(dto.ErrorMessage);
            }
            catch
            {
                _statusCode = -1;
                try
                {
                    var r = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(r))
                        _errorMessage = new MarkupString(r);
                    //ToDo label
                    else
                        _errorMessage = new MarkupString("Opps, something went wrong");
                }
                catch (Exception ex)
                {
                    _errorMessage = new MarkupString($"Exception occurred: {ex.Message}");
                }
            }

            _isLoading = false;
            _isError = true;
            return default;
        }

        protected async Task<HttpResponseMessage> PostAsync<T>(string url, _BaseDto<T> dto) where T : _BaseDto<T>
        {
            _isSaving = true;
            var client = await GetClient();

            var json = System.Text.Json.JsonSerializer.Serialize((T)dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            _isSaving = false;
            return response;
        }

        protected async Task<_ResponseDto> PostAsync<T>(string url, IEnumerable<T> dtos) where T : _BaseFieldsDto<T>
        {
            _isSaving = true;
            _isValidationError = false;

            foreach (var d in dtos)
                d.IsError = false;

            var client = await GetClient();

            var json = System.Text.Json.JsonSerializer.Serialize(dtos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            var r = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<_ResponseDto>(r);

            if (dto.StatusCode == GC.StatusCodeUnProcessable)
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
