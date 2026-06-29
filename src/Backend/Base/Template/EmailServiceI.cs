namespace Backend.Base.Template
{
    public interface EmailServiceI
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
