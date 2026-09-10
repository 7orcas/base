using Microsoft.AspNetCore.Http;
using System.Collections;


namespace Common.DTO
{
    public class _ResponseDto
    {
        public _ResponseDto() { }

        public _ResponseDto(IList list) 
        {
            SuccessMessage = "Ok";
            RecordCount = list.Count;
            Result = list;
        }

        public int StatusCode { get; set; } = StatusCodes.Status200OK;
        public bool Valid { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public int? RecordCount { get; set; }
        public object? Result { get; set; }

        public List<ValidationDto>? Validations { get; set; } 
    }

    public class ValidationDto
    {
        public long Id { get; set; }
        public List<ValidationMessageDto>? Messages { get; set; }
    }

    public class ValidationMessageDto
    {
        public string Message { get; set; }
        public bool IsWarning { get; set; } = false;
        public bool IsError { get; set; } = false;
    }

}
