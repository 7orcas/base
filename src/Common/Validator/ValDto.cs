using GC = Common.GlobalConstants;

/// <summary>
/// Dto validation message(s)
/// Created: July 2025
/// [*Licence*]
/// Author: John Stewart
/// </summary>

namespace Common.Validator
{
    public class ValDto
    {
        public long Id { get; set; }
        public string Code { get; set; }

        public List<ValMessage> Messages { get; set; } = new List<ValMessage>();

        public int Status()
        {
            int s = GC.ValStatusOk;

            if (Messages == null) return s;
            foreach (ValMessage m in Messages)
            {
                if (m.Status == GC.ValStatusError)
                    return GC.ValStatusError;
            }
            return s;
        }

        public void AddError(string field, string message) => Add(field, message, GC.ValStatusError);

        private void Add(string field, string message, int status)
        {
            Messages.Add(new ValMessage
            {
                Status = status,
                Field = field,
                Message = message
            });
        }

        public bool IsError () => Messages != null && Messages.Count > 0;

    }

    public class ValMessage 
    { 
        public int Status { get; set; }
        public string Field { get; set; }
        public string Message { get; set; }
    }

}
