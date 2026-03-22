using EXE_BE.Application.DTOs.Requests.Payment;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.Payment;

namespace EXE_BE.Application.IServices
{
    public interface IPaymentService
    {
        Task<ApiResponse> CreatePayOSPaymentLinkAsync(CreatePayOSPaymentLinkRequest request);
        Task<ApiResponse> ConfirmPayOSPaymentAsync(ConfirmPayOSPaymentRequest request);
        Task<ApiResponse> HandlePayOSWebhookAsync(PayOSWebhookRequest request);
        Task<ApiResponse> GetInvoicePdfAsync(long orderCode);
    }
}
