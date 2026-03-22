using System.Net;
using System.Text.Json.Serialization;

namespace EXE_BE.Application.DTOs.Responses
{
    public class ApiResponse
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; private set; }

        public bool Success { get; private set; }
        public string Code { get; private set; } = ((int)HttpStatusCode.OK).ToString();
        public string Message { get; private set; } = string.Empty;
        public object? Data { get; private set; }

        [JsonIgnore]
        public bool IsSuccess => Success;

        [JsonIgnore]
        public string? ErrorMessage => Message;

        [JsonIgnore]
        public object? Result => Data;

        public ApiResponse SetOk(object? result = null, string? message = null)
        {
            StatusCode = HttpStatusCode.OK;
            Success = true;
            Code = ((int)HttpStatusCode.OK).ToString();
            Message = string.IsNullOrWhiteSpace(message) ? "Thành công." : message;
            Data = result;
            return this;
        }

        public ApiResponse SetNotFound(object? result = null, string? message = null)
        {
            StatusCode = HttpStatusCode.NotFound;
            Success = false;
            Code = ((int)HttpStatusCode.NotFound).ToString();
            Message = string.IsNullOrWhiteSpace(message) ? "Không tìm thấy dữ liệu." : message;
            Data = result;
            return this;
        }

        public ApiResponse SetBadRequest(object? result = null, string? message = null)
        {
            StatusCode = HttpStatusCode.BadRequest;
            Success = false;
            Code = ((int)HttpStatusCode.BadRequest).ToString();
            Message = string.IsNullOrWhiteSpace(message) ? "Yêu cầu không hợp lệ." : message;
            Data = result;
            return this;
        }

        public ApiResponse SetApiResponse(HttpStatusCode statusCode, bool isSuccess, string? message = null, object? result = null)
        {
            StatusCode = statusCode;
            Success = isSuccess;
            Code = ((int)statusCode).ToString();
            Message = string.IsNullOrWhiteSpace(message)
                ? (isSuccess ? "Thành công." : "Có lỗi xảy ra.")
                : message;
            Data = result;
            return this;
        }
    }
}
