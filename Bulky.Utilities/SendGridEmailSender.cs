using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BulkyBook.Utilities;
public class SendGridEmailSender : IEmailSender
{
    private string SecretKey { get; set; }

    public SendGridEmailSender(IConfiguration configuration)
    {
        SecretKey = configuration.GetValue<string>("SendGrid:SecretKey");
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
        //var client = new SendGridClient(SecretKey);

        //var from = new EmailAddress("admin@finazzy.com", "Bulky Book");
        //var to = new EmailAddress(email);

        //var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

        //return client.SendEmailAsync(message);
    }
}
