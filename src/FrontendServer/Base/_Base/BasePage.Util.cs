using FrontendServer.Base.User;
using MudBlazor;
using Microsoft.AspNetCore.Components;

namespace FrontendServer.Base._Base
{
    public partial class BasePage
    {
        public async Task<DialogResult> Search<T>() where T : IComponent
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = false,
                BackdropClick = false
            };

            var dialog = await DialogService.ShowAsync<T>("", options);
            return await dialog.Result;

        }


    }
}
