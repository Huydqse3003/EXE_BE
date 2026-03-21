using EXE_BE.Application.DTOs.Requests.Friendship;
using EXE_BE.API.Hubs;
using EXE_BE.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EXE_BE.API.Controllers
{
    [ApiController]
    [Route("api/friendships")]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IHubContext<RealtimeHub> _hubContext;

        public FriendshipController(IFriendshipService friendshipService, IHubContext<RealtimeHub> hubContext)
        {
            _friendshipService = friendshipService;
            _hubContext = hubContext;
        }

        [HttpPost("request")]
        public async Task<IActionResult> SendRequest([FromBody] CreateFriendshipRequest request)
        {
            try
            {
                var result = await _friendshipService.AddAsync(request);
                await _hubContext.Clients.Group($"user:{request.FriendId}")
                    .SendAsync("FriendRequestReceived", result);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{friendshipId:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid friendshipId, [FromBody] UpdateFriendshipRequest request)
        {
            try
            {
                var result = await _friendshipService.UpdateAsync(friendshipId, request);
                if (result == null)
                {
                    return NotFound("Không tìm thấy lời mời kết bạn.");
                }

                await _hubContext.Clients.Group($"user:{result.UserId}")
                    .SendAsync("FriendRequestStatusUpdated", result);
                await _hubContext.Clients.Group($"user:{result.FriendId}")
                    .SendAsync("FriendRequestStatusUpdated", result);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("pending/{userId:guid}")]
        public async Task<IActionResult> GetPending(Guid userId)
        {
            try
            {
                var result = await _friendshipService.GetPendingByUserIdAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
