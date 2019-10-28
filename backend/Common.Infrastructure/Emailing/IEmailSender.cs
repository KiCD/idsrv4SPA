using System.Threading.Tasks;

namespace Common.Infrastructure.Emailing
{
    public interface IEmailSender
    {
        Task SendEmail(string emailAddress, string subject, string htmlMessage);
    }
}
