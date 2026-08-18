using Microsoft.AspNetCore.Components;

namespace FrontendServer.Base.Util
{
    public class LoadStatus
    {
        public bool IsLoading { get; set; } = false;
        public bool IsError { get; set; } = false;
        public bool IsSaving { get; set; } = false;
        public bool IsSearch { get; set; } = false;
        public int? StatusCode { get; set; }
        public MarkupString? ErrorMessage { get; set; }
        public MarkupString? Message { get; set; }
        public Exception? Exception { get; set; }
        

        public Boolean Show()
        {
            return IsLoading || IsError || IsSaving || IsSearch;
        }


        //Convenience methods
        public LoadStatus SetLoading()
        {
            IsLoading = true;
            return this;
        }

        public LoadStatus ResetLoading()
        {
            IsLoading = false;
            return this;
        }

        public LoadStatus SetSaving()
        {
            IsSaving = true;
            return this;
        }
        public LoadStatus ResetSaving()
        {
            IsSaving = false;
            return this;
        }

        public LoadStatus SetSearch()
        {
            IsSearch = true;
            return this;
        }
        public LoadStatus ResetSearch()
        {
            IsSearch = false;
            return this;
        }

        public LoadStatus SetError()
        {
            IsError = true;
            return this;
        }

        public LoadStatus SetErrorMessage(MarkupString? errorMessage)
        {
            ErrorMessage = errorMessage;
            return this;
        }

        public LoadStatus SetStatusCode(int? statusCode)
        {
            StatusCode = statusCode;
            return this;
        }

    }
}
