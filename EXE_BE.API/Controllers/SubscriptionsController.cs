using EXE_BE.Application.DTOs.Requests.Payment;
using EXE_BE.Application.DTOs.Requests.SubscriptionPackage;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.Payment;
using EXE_BE.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_BE.API.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionPackageService _subscriptionPackageService;
        private readonly IPaymentService _paymentService;

        public SubscriptionsController(ISubscriptionPackageService subscriptionPackageService, IPaymentService paymentService)
        {
            _subscriptionPackageService = subscriptionPackageService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Lấy danh sách toàn bộ gói đăng ký.
        /// </summary>
        [HttpGet("packages")]
        public async Task<IActionResult> GetPackages()
        {
            var response = await _subscriptionPackageService.GetAllAsync();
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Lấy chi tiết một gói đăng ký theo id.
        /// </summary>
        [HttpGet("packages/{packageId:guid}")]
        public async Task<IActionResult> GetPackageById(Guid packageId)
        {
            var response = await _subscriptionPackageService.GetByIdAsync(packageId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Tạo gói đăng ký mới.
        /// </summary>
        [HttpPost("packages")]
        public async Task<IActionResult> CreatePackage([FromBody] CreateSubscriptionPackageRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse().SetBadRequest(message: string.Join("; ", errors)));
            }

            var response = await _subscriptionPackageService.AddAsync(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Tạo link thanh toán PayOS cho gói đăng ký.
        /// </summary>
        [HttpPost("payos/create-link")]
        public async Task<IActionResult> CreatePayOSLink([FromBody] CreatePayOSPaymentLinkRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse().SetBadRequest(message: string.Join("; ", errors)));
            }

            var response = await _paymentService.CreatePayOSPaymentLinkAsync(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Xác nhận thanh toán PayOS và kích hoạt gói.
        /// </summary>
        [HttpPost("payos/confirm")]
        public async Task<IActionResult> ConfirmPayOS([FromBody] ConfirmPayOSPaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse().SetBadRequest(message: string.Join("; ", errors)));
            }

            var response = await _paymentService.ConfirmPayOSPaymentAsync(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Endpoint nhận webhook thanh toán từ PayOS.
        /// </summary>
        [HttpPost("payos/webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PayOSWebhook([FromBody] PayOSWebhookRequest request)
        {
            var response = await _paymentService.HandlePayOSWebhookAsync(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Tải file PDF invoice theo mã đơn hàng PayOS.
        /// </summary>
        [HttpGet("payos/invoices/{orderCode:long}")]
        public async Task<IActionResult> DownloadInvoice(long orderCode)
        {
            var response = await _paymentService.GetInvoicePdfAsync(orderCode);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            if (response.Result is not InvoicePdfResponse invoice)
            {
                return BadRequest(response);
            }

            return File(invoice.Content, "application/pdf", invoice.FileName);
        }
    }
}
