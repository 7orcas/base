namespace FrontendServer.Base.Util
{
    public class ValidationModel
    {

        public ValidationDto Validation { get; set; }

        public List<ValidationMessageDto> Messages => Validation.Messages;

        public int MessageCount => Validation.Messages.Count;

        public bool IsValidation() => Validation != null;
        public bool IsValidationError()
        {
            for (int i = 0; Validation != null && i < Validation.Messages.Count; i++)
                if (Validation.Messages[i].IsError) return true;
            return false;
        }
        public bool IsValidationWarning()
        {
            for (int i = 0; Validation != null && i < Validation.Messages.Count; i++)
                if (Validation.Messages[i].IsWarning) return true;
            return false;
        }

    }
}
