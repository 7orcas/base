namespace Backend.Base.Email
{
    public interface EmailServiceI
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
