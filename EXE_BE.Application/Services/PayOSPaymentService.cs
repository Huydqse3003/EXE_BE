using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EXE_BE.Application.DTOs.Requests.Payment;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.Payment;
using EXE_BE.Application.IServices;
using EXE_BE.Domain;
using EXE_BE.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EXE_BE.Application.Services
{
    public class PayOSPaymentService : IPaymentService
    {
        private static bool _questPdfLicenseRegistered;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        private readonly PayOSSettings _payOSOptions;
        private readonly IEmailService _emailService;

        public PayOSPaymentService(IUnitOfWork unitOfWork, HttpClient httpClient, AppSettings appSettings, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
            _payOSOptions = appSettings.PayOS;
            _emailService = emailService;

            if (!_questPdfLicenseRegistered)
            {
                QuestPDF.Settings.License = LicenseType.Community;
                _questPdfLicenseRegistered = true;
            }
        }

        public async Task<ApiResponse> CreatePayOSPaymentLinkAsync(CreatePayOSPaymentLinkRequest request)
        {
            var response = new ApiResponse();
            try
            {
                ValidatePayOSConfiguration();

                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
                    ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

                var package = await _unitOfWork.SubscriptionPackages.GetByIdAsync(request.PackageId)
                    ?? throw new KeyNotFoundException("Không tìm thấy gói đăng ký.");

                if (package.Price <= 0)
                {
                    throw new InvalidOperationException("Gói đăng ký chưa có giá hợp lệ để thanh toán.");
                }

                var amount = (int)Math.Round(package.Price, MidpointRounding.AwayFromZero);
                var orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var now = DateTime.UtcNow;

                var paymentTransaction = new PaymentTransaction
                {
                    PaymentTransactionId = Guid.NewGuid(),
                    OrderCode = orderCode,
                    UserId = user.UserId,
                    PackageId = package.PackageId,
                    Amount = package.Price,
                    Status = "CREATED",
                    CreatedAt = now
                };

                await _unitOfWork.PaymentTransactions.AddAsync(paymentTransaction);
                await _unitOfWork.SaveChangesAsync();

                var returnUrl = string.IsNullOrWhiteSpace(request.ReturnUrl) ? _payOSOptions.ReturnUrl : request.ReturnUrl.Trim();
                var cancelUrl = string.IsNullOrWhiteSpace(request.CancelUrl) ? _payOSOptions.CancelUrl : request.CancelUrl.Trim();
                var description = BuildDescription(package.Name);

                var signatureContent = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
                var signature = ComputeHmacSha256(signatureContent, _payOSOptions.ChecksumKey);

                var payload = new
                {
                    orderCode,
                    amount,
                    description,
                    returnUrl,
                    cancelUrl,
                    buyerName = user.Username,
                    buyerEmail = user.Email,
                    buyerPhone = user.PhoneNumber,
                    signature
                };

                var requestContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_payOSOptions.BaseUrl.TrimEnd('/')}/v2/payment-requests")
                {
                    Content = requestContent
                };

                httpRequest.Headers.Add("x-client-id", _payOSOptions.ClientId);
                httpRequest.Headers.Add("x-api-key", _payOSOptions.ApiKey);

                var httpResponse = await _httpClient.SendAsync(httpRequest);
                var responseText = await httpResponse.Content.ReadAsStringAsync();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Không tạo được link thanh toán PayOS. {responseText}");
                }

                using var document = JsonDocument.Parse(responseText);
                var root = document.RootElement;
                var code = root.TryGetProperty("code", out var codeElement) ? codeElement.GetString() : null;

                if (code != "00")
                {
                    var desc = root.TryGetProperty("desc", out var descElement) ? descElement.GetString() : "Lỗi từ PayOS.";
                    throw new InvalidOperationException(desc ?? "Lỗi từ PayOS.");
                }

                if (!root.TryGetProperty("data", out var dataElement))
                {
                    throw new InvalidOperationException("Không đọc được dữ liệu thanh toán từ PayOS.");
                }

                paymentTransaction.PaymentLinkId = dataElement.TryGetProperty("paymentLinkId", out var pId) ? pId.GetString() : null;
                paymentTransaction.Status = dataElement.TryGetProperty("status", out var pStatus) ? pStatus.GetString() ?? "CREATED" : "CREATED";
                _unitOfWork.PaymentTransactions.Update(paymentTransaction);
                await _unitOfWork.SaveChangesAsync();

                return response.SetOk(new CreatePayOSPaymentLinkResponse
                {
                    OrderCode = orderCode,
                    Amount = package.Price,
                    CheckoutUrl = dataElement.TryGetProperty("checkoutUrl", out var checkoutUrl) ? checkoutUrl.GetString() ?? string.Empty : string.Empty,
                    QrCode = dataElement.TryGetProperty("qrCode", out var qrCode) ? qrCode.GetString() ?? string.Empty : string.Empty,
                    PaymentLinkId = dataElement.TryGetProperty("paymentLinkId", out var paymentLinkId) ? paymentLinkId.GetString() ?? string.Empty : string.Empty,
                    Status = dataElement.TryGetProperty("status", out var status) ? status.GetString() ?? string.Empty : string.Empty
                });
            }
            catch (KeyNotFoundException ex)
            {
                return response.SetNotFound(message: ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return response.SetBadRequest(message: ex.Message);
            }
        }

        public async Task<ApiResponse> ConfirmPayOSPaymentAsync(ConfirmPayOSPaymentRequest request)
        {
            var response = new ApiResponse();
            try
            {
            if (!request.IsPaid)
            {
                return response.SetBadRequest(new ConfirmPayOSPaymentResponse
                {
                    IsSuccess = false,
                    Message = string.IsNullOrWhiteSpace(request.Status)
                        ? "Thanh toán chưa thành công."
                        : $"Thanh toán chưa thành công: {request.Status}"
                });
            }

            var transaction = (await _unitOfWork.PaymentTransactions.FindAsync(t => t.OrderCode == request.OrderCode))
                .FirstOrDefault();

            if (transaction?.Status == "PAID")
            {
                return response.SetOk(new ConfirmPayOSPaymentResponse
                {
                    IsSuccess = true,
                    Message = "Giao dịch đã được xác nhận trước đó.",
                    OrderCode = request.OrderCode
                });
            }

            var userId = transaction?.UserId ?? request.UserId;
            var packageId = transaction?.PackageId ?? request.PackageId;

            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var package = await _unitOfWork.SubscriptionPackages.GetByIdAsync(packageId)
                ?? throw new KeyNotFoundException("Không tìm thấy gói đăng ký.");

            var now = DateTime.UtcNow;
            var latestSubscription = (await _unitOfWork.UserSubscriptions.FindAsync(s =>
                    s.UserId == request.UserId &&
                    s.PackageId == request.PackageId))
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefault();

            var startDate = latestSubscription != null && latestSubscription.EndDate > now
                ? latestSubscription.EndDate
                : now;

            var endDate = startDate.AddDays(package.DurationDays);

            var userSubscription = new UserSubscription
            {
                UserSubscriptionId = Guid.NewGuid(),
                UserId = user.UserId,
                PackageId = package.PackageId,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true
            };

            await _unitOfWork.UserSubscriptions.AddAsync(userSubscription);

            var invoiceFilePath = await GenerateInvoicePdfAsync(user, package, request.OrderCode, request.TransactionId, userSubscription.StartDate, userSubscription.EndDate, now, package.Price);

            var emailSentMessage = "";
            try
            {
                var subject = $"Invoice thanh toán gói {package.Name} - #{request.OrderCode}";
                var body = BuildInvoiceEmailBody(user.Username, package.Name, request.OrderCode, package.Price, userSubscription.StartDate, userSubscription.EndDate);
                await _emailService.SendInvoiceEmailAsync(user.Email, user.Username, subject, body, invoiceFilePath);
                emailSentMessage = " Invoice đã được gửi qua email.";
            }
            catch
            {
                emailSentMessage = " Không thể gửi invoice qua email, vui lòng tải invoice từ hệ thống.";
            }

            if (transaction == null)
            {
                transaction = new PaymentTransaction
                {
                    PaymentTransactionId = Guid.NewGuid(),
                    OrderCode = request.OrderCode,
                    UserId = user.UserId,
                    PackageId = package.PackageId,
                    Amount = package.Price,
                    CreatedAt = now
                };

                await _unitOfWork.PaymentTransactions.AddAsync(transaction);
            }

            transaction.Status = "PAID";
            transaction.TransactionId = request.TransactionId;
            transaction.PaidAt = now;
            transaction.InvoiceFilePath = invoiceFilePath;
            _unitOfWork.PaymentTransactions.Update(transaction);

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = "SubscriptionPayment",
                EventTime = now,
                Details = $"Paid subscription package {package.Name} - OrderCode: {request.OrderCode} - Txn: {request.TransactionId}"
            });

            await _unitOfWork.SaveChangesAsync();

            return response.SetOk(new ConfirmPayOSPaymentResponse
            {
                IsSuccess = true,
                Message = "Xác nhận thanh toán thành công và đã kích hoạt gói." + emailSentMessage,
                UserSubscriptionId = userSubscription.UserSubscriptionId,
                StartDate = userSubscription.StartDate,
                EndDate = userSubscription.EndDate,
                OrderCode = request.OrderCode
            });
            }
            catch (KeyNotFoundException ex)
            {
                return response.SetNotFound(message: ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return response.SetBadRequest(message: ex.Message);
            }
        }

        public async Task<ApiResponse> HandlePayOSWebhookAsync(PayOSWebhookRequest request)
        {
            var response = new ApiResponse();
            try
            {
            if (request.Data == null)
            {
                throw new InvalidOperationException("Webhook PayOS không hợp lệ.");
            }

            if (!request.Success || request.Code != "00")
            {
                return response.SetBadRequest(new ConfirmPayOSPaymentResponse
                {
                    IsSuccess = false,
                    Message = string.IsNullOrWhiteSpace(request.Desc) ? "Thanh toán chưa thành công." : request.Desc,
                    OrderCode = request.Data.OrderCode
                });
            }

            var transaction = (await _unitOfWork.PaymentTransactions.FindAsync(t => t.OrderCode == request.Data.OrderCode))
                .FirstOrDefault()
                ?? throw new KeyNotFoundException("Không tìm thấy giao dịch cần xác nhận.");

            return await ConfirmPayOSPaymentAsync(new ConfirmPayOSPaymentRequest
            {
                UserId = transaction.UserId,
                PackageId = transaction.PackageId,
                OrderCode = request.Data.OrderCode,
                IsPaid = true,
                Status = request.Data.Status,
                TransactionId = request.Data.Reference
            });
            }
            catch (KeyNotFoundException ex)
            {
                return response.SetNotFound(message: ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return response.SetBadRequest(message: ex.Message);
            }
        }

        public async Task<ApiResponse> GetInvoicePdfAsync(long orderCode)
        {
            var response = new ApiResponse();
            var transaction = (await _unitOfWork.PaymentTransactions.FindAsync(t => t.OrderCode == orderCode))
                .FirstOrDefault();

            if (transaction == null || string.IsNullOrWhiteSpace(transaction.InvoiceFilePath))
            {
                return response.SetNotFound(message: "Không tìm thấy invoice cho đơn hàng này.");
            }

            if (!File.Exists(transaction.InvoiceFilePath))
            {
                return response.SetNotFound(message: "Không tìm thấy file invoice.");
            }

            return response.SetOk(new InvoicePdfResponse
            {
                Content = await File.ReadAllBytesAsync(transaction.InvoiceFilePath),
                FileName = Path.GetFileName(transaction.InvoiceFilePath)
            });
        }

        private void ValidatePayOSConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_payOSOptions.ClientId) ||
                string.IsNullOrWhiteSpace(_payOSOptions.ApiKey) ||
                string.IsNullOrWhiteSpace(_payOSOptions.ChecksumKey) ||
                string.IsNullOrWhiteSpace(_payOSOptions.ReturnUrl) ||
                string.IsNullOrWhiteSpace(_payOSOptions.CancelUrl))
            {
                throw new InvalidOperationException("Thiếu cấu hình PayOS trong appsettings.");
            }
        }

        private static string BuildDescription(string packageName)
        {
            var description = $"SUB-{packageName}".Trim();
            return description.Length <= 25 ? description : description[..25];
        }

        private static string BuildInvoiceEmailBody(string username, string packageName, long orderCode, decimal amount, DateTime startDate, DateTime endDate)
        {
            return $"""
                <p>Xin chào {username},</p>
                <p>Bạn đã thanh toán thành công gói <strong>{packageName}</strong>.</p>
                <ul>
                    <li>Mã đơn hàng: <strong>{orderCode}</strong></li>
                    <li>Số tiền: <strong>{amount:N0} VND</strong></li>
                    <li>Hiệu lực: <strong>{startDate:yyyy-MM-dd HH:mm:ss} UTC</strong> đến <strong>{endDate:yyyy-MM-dd HH:mm:ss} UTC</strong></li>
                </ul>
                <p>File PDF invoice được đính kèm trong email này.</p>
                <p>Trân trọng.</p>
                """;
        }

        private static async Task<string> GenerateInvoicePdfAsync(
            User user,
            SubscriptionPackage package,
            long orderCode,
            string? transactionId,
            DateTime startDate,
            DateTime endDate,
            DateTime paidAt,
            decimal amount)
        {
            var invoiceDirectory = Path.Combine(AppContext.BaseDirectory, "invoices");
            Directory.CreateDirectory(invoiceDirectory);

            var fileName = $"invoice-{orderCode}.pdf";
            var fullPath = Path.Combine(invoiceDirectory, fileName);

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.Header().Text("PAYMENT INVOICE").FontSize(20).Bold();

                    page.Content().Column(column =>
                    {
                        column.Spacing(8);
                        column.Item().Text($"Invoice No: {orderCode}");
                        column.Item().Text($"Transaction Id: {transactionId ?? "N/A"}");
                        column.Item().Text($"Paid At (UTC): {paidAt:yyyy-MM-dd HH:mm:ss}");
                        column.Item().Text($"Customer: {user.Username} ({user.Email})");
                        column.Item().Text($"Package: {package.Name}");
                        column.Item().Text($"Amount: {amount:N0} VND");
                        column.Item().Text($"Subscription Start: {startDate:yyyy-MM-dd HH:mm:ss} UTC");
                        column.Item().Text($"Subscription End: {endDate:yyyy-MM-dd HH:mm:ss} UTC");
                    });
                });
            }).GeneratePdf();

            await File.WriteAllBytesAsync(fullPath, pdfBytes);
            return fullPath;
        }

        private static string ComputeHmacSha256(string rawData, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}
