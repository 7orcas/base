using GC = Common.GlobalConstants;


namespace Common.DTO
{
    public class _ResponseDto
    {
        public int StatusCode { get; set; } = GC.StatusCodeOk;
        public bool Valid { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public object? Result { get; set; }

    }
}
