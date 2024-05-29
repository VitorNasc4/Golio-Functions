using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GolioFunctions.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public string Provedor { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            Provedor = _configuration["SuggestionNotification:Provedor"]!;
            Username = _configuration["SuggestionNotification:Username"]!;
            Password = _configuration["SuggestionNotification:Password"]!;
        }

        public void SendEmail(string emailTo, string subject, string body)
        {
            var message = PrepareteMessage(emailTo, subject, body);

            SendEmailBySmtp(message);
        }

        private MailMessage PrepareteMessage(string emailTo, string subject, string body)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(Username)
            };

            if (ValidateEmail(emailTo))
            {
                mail.To.Add(emailTo);
            }

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            return mail;
        }

        private bool ValidateEmail(string email)
        {
            Regex expression = new Regex(@"\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}");
            if (expression.IsMatch(email))
            {
                return true;
            }

            return false;
        }

        private void SendEmailBySmtp(MailMessage message)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Host = Provedor,
                    Port = 587,
                    EnableSsl = true,
                    Timeout = 50000,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Username, Password)
                };
                smtpClient.Send(message);
                smtpClient.Dispose();
                Console.WriteLine("Email sucessfully sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

    }
}