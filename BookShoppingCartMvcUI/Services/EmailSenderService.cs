using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace BookShoppingCartMvcUI.Services
{
    public class EmailSenderService : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = new MailMessage
            {
                From = new MailAddress("bookshoppingcart@gmail.com", "BookShoppingCart"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mail.To.Add(email);

            using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("bookshoppingcart@gmail.com", "ymhmjapntrrxpmne"),
                EnableSsl = true,
                 DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
            };

            await smtpClient.SendMailAsync(mail);
        }
    }
}