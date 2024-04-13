using Org.BouncyCastle.Asn1.Pkcs;
using WebApplication1.Model;

namespace WebApplication1.Service
{
    public interface ISendMailService
    {
        Task SendMail(MailContent mailContent);

        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
