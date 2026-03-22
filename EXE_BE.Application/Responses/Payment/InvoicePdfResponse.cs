namespace EXE_BE.Application.DTOs.Responses.Payment
{
    public class InvoicePdfResponse
    {
        public byte[] Content { get; set; } = [];
        public string FileName { get; set; } = string.Empty;
    }
}
