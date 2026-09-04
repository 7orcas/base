using FrontendServer.Base.Component;
using FrontendServer.Base.User;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FrontendServer.Base._Base
{
    public partial class BasePage
    {
        public async Task<DialogResult> OpenSearchDialog<T>(int? maxRecordsReturned = 200) where T : IComponent
        {
            var parameters = new DialogParameters
                {
                    { nameof(BaseSearchDialog.MaxRecordsReturned), maxRecordsReturned },
                    { nameof(BaseSearchDialog.IsService), _config.User.IsService }
                };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = false,
                BackdropClick = false
            };
            var dialog = await DialogService.ShowAsync<T>("", parameters, options);
            return await dialog.Result;
        }


        public async Task<DialogResult> OpenConfirmYesNoDialog() 
        {
            var parameters = new DialogParameters
                {
                    { nameof(ConfirmYesNoDialog.Message), GetLabel("UnsavedS") }
                };

            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Small
            };

            var dialog = await DialogService.ShowAsync<ConfirmYesNoDialog>(
                GetLabel("Confirm"),
                parameters,
                options);
            return await dialog.Result;
        }


    }
}
