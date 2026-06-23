namespace Backend.Base.Template
{
    public interface TemplateServiceI
    {
        Task<string?> GetResetRequestEmail(LoginEnt login);
    }
}
