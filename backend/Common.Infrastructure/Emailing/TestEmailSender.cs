using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure.Emailing
{
    public class TestEmailSender : IEmailSender
    {
        private readonly Random _random = new Random();
        public Task SendEmail(string emailAddress, string subject, string htmlMessage)
        {
            return File.WriteAllTextAsync($"{subject} {_random.Next(1,10000)}.html", htmlMessage);
        }
    }
}
