namespace Common.Validator
{
    public interface ValidatorI<T>
    {
        ValDto Validate(T dto, int? orgNr, Dictionary<string, string> labels);
        bool IsCodeUnique();
    }
}
