using System.Net;
using System.Net.Mail;
using EXE_BE.Application.IServices;
using EXE_BE.Domain;

namespace EXE_BE.API.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpEmailSettings _smtpOptions;

        public SmtpEmailService(AppSettings appSettings)
        {
            _smtpOptions = appSettings.SmtpEmail;
        }

        public async Task SendInvoiceEmailAsync(string toEmail, string recipientName, string subject, string bodyHtml, string attachmentFilePath)
        {
            if (string.IsNullOrWhiteSpace(_smtpOptions.Host) ||
                string.IsNullOrWhiteSpace(_smtpOptions.Username) ||
                string.IsNullOrWhiteSpace(_smtpOptions.Password) ||
                string.IsNullOrWhiteSpace(_smtpOptions.FromEmail))
            {
                throw new InvalidOperationException("Thiếu cấu hình SMTP trong appsettings.");
            }

            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new InvalidOperationException("Email người nhận không hợp lệ.");
            }

            if (!File.Exists(attachmentFilePath))
            {
                throw new FileNotFoundException("Không tìm thấy file invoice để gửi email.", attachmentFilePath);
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_smtpOptions.FromEmail, _smtpOptions.FromName),
                Subject = subject,
                Body = bodyHtml,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toEmail, recipientName));
            message.Attachments.Add(new Attachment(attachmentFilePath));

            using var smtpClient = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port)
            {
                EnableSsl = _smtpOptions.EnableSsl,
                Credentials = new NetworkCredential(_smtpOptions.Username, _smtpOptions.Password)
            };

            await smtpClient.SendMailAsync(message);
        }
    }
}
