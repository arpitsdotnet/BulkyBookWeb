using Microsoft.AspNetCore.Identity.UI.Services;

namespace BulkyBook.Utilities;
public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}
