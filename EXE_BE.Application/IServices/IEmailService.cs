namespace EXE_BE.Application.IServices
{
    public interface IEmailService
    {
        Task SendInvoiceEmailAsync(string toEmail, string recipientName, string subject, string bodyHtml, string attachmentFilePath);
    }
}
