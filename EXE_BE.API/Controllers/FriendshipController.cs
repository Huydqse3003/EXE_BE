using EXE_BE.Application.DTOs.Requests.Friendship;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.Friendship;
using EXE_BE.API.Hubs;
using EXE_BE.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EXE_BE.API.Controllers
{
    [ApiController]
    [Route("api/friendships")]
    [Authorize]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IHubContext<RealtimeHub> _hubContext;

        public FriendshipController(IFriendshipService friendshipService, IHubContext<RealtimeHub> hubContext)
        {
            _friendshipService = friendshipService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Gửi lời mời kết bạn.
        /// </summary>
        [HttpPost("request")]
        public async Task<IActionResult> SendRequest([FromBody] CreateFriendshipRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse().SetBadRequest(message: string.Join("; ", errors)));
            }

            var response = await _friendshipService.AddAsync(request);
            if (response.IsSuccess && response.Result is FriendshipResponse friendship)
            {
                await _hubContext.Clients.Group($"user:{request.FriendId}")
                    .SendAsync("FriendRequestReceived", friendship);
            }

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Cập nhật trạng thái lời mời kết bạn (chấp nhận/từ chối).
        /// </summary>
        [HttpPut("{friendshipId:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid friendshipId, [FromBody] UpdateFriendshipRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse().SetBadRequest(message: string.Join("; ", errors)));
            }

            var response = await _friendshipService.UpdateAsync(friendshipId, request);
            if (response.IsSuccess && response.Result is FriendshipResponse friendship)
            {
                await _hubContext.Clients.Group($"user:{friendship.UserId}")
                    .SendAsync("FriendRequestStatusUpdated", friendship);
                await _hubContext.Clients.Group($"user:{friendship.FriendId}")
                    .SendAsync("FriendRequestStatusUpdated", friendship);
            }

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Lấy danh sách lời mời kết bạn đang chờ của người dùng.
        /// </summary>
        [HttpGet("pending/{userId:guid}")]
        public async Task<IActionResult> GetPending(Guid userId)
        {
            var response = await _friendshipService.GetPendingByUserIdAsync(userId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
