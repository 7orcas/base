using Common.Search;
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

        public _ResponseDto(IList listBefore, IList listUpdated)
        {
            SuccessMessage = "Ok";
            RecordCount = listUpdated.Count;
            Result = listUpdated;
            AuditObject = listBefore;
        }

        public _ResponseDto(_BaseDto dto)
        {
            SuccessMessage = "Ok";
            RecordCount = 1;
            Result = dto;
        }

        public _ResponseDto(_BaseSearch search)
        {
            SuccessMessage = "Ok";
            Result = search;
        }

        public int StatusCode { get; set; } = StatusCodes.Status200OK;
        public bool Valid { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public int? RecordCount { get; set; }
        public object? Result { get; set; }
        public object? AuditObject { get; set; }

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
