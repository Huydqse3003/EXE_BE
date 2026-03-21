using EXE_BE.Application.DTOs.Requests.Gameplay;
using EXE_BE.API.Hubs;
using EXE_BE.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EXE_BE.API.Controllers
{
    [ApiController]
    [Route("api/gameplay")]
    public class GameplayController : ControllerBase
    {
        private readonly IGameplayService _gameplayService;
        private readonly IHubContext<RealtimeHub> _hubContext;

        public GameplayController(IGameplayService gameplayService, IHubContext<RealtimeHub> hubContext)
        {
            _gameplayService = gameplayService;
            _hubContext = hubContext;
        }

        [HttpPost("focus-sessions/start")]
        public async Task<IActionResult> StartFocusSession([FromBody] StartFocusSessionRequest request)
        {
            try
            {
                var result = await _gameplayService.StartFocusSessionAsync(request);
                await _hubContext.Clients.Group($"user:{request.UserId}")
                    .SendAsync("FocusSessionStarted", result);
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

        [HttpPost("focus-sessions/{sessionId:guid}/complete")]
        public async Task<IActionResult> CompleteFocusSession(Guid sessionId)
        {
            try
            {
                var result = await _gameplayService.CompleteFocusSessionAsync(sessionId);
                await _hubContext.Clients.Group($"user:{result.UserId}")
                    .SendAsync("FocusSessionCompleted", result);
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

        [HttpGet("shop/items")]
        public async Task<IActionResult> GetShopItems()
        {
            var result = await _gameplayService.GetShopItemsAsync();
            return Ok(result);
        }

        [HttpPost("shop/purchase")]
        public async Task<IActionResult> BuyItem([FromBody] BuyShopItemRequest request)
        {
            try
            {
                var result = await _gameplayService.BuyItemWithCoinsAsync(request);
                await _hubContext.Clients.Group($"user:{request.UserId}")
                    .SendAsync("ShopItemPurchased", result);
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

        [HttpPost("rooms/decorate")]
        public async Task<IActionResult> DecorateRoom([FromBody] DecorateVirtualRoomRequest request)
        {
            try
            {
                var result = await _gameplayService.DecorateRoomAsync(request);
                await _hubContext.Clients.Group($"virtual-room:{request.VirtualRoomId}")
                    .SendAsync("RoomDecorated", result);
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

        [HttpPost("rooms/join-friend")]
        public async Task<IActionResult> JoinFriendRoom([FromBody] JoinFriendRoomRequest request)
        {
            try
            {
                var result = await _gameplayService.JoinFriendRoomAsync(request);
                await _hubContext.Clients.Group($"virtual-room:{result.VirtualRoomId}")
                    .SendAsync("FriendJoinedRoom", result);
                await _hubContext.Clients.Group($"user:{result.FriendUserId}")
                    .SendAsync("FriendJoinedYourRoom", result);
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

        [HttpPost("study-rooms")]
        public async Task<IActionResult> CreateStudyRoom([FromBody] CreateStudyRoomFlowRequest request)
        {
            try
            {
                var result = await _gameplayService.CreateStudyRoomAsync(request);
                await _hubContext.Clients.Group($"user:{request.CreatedByUserId}")
                    .SendAsync("StudyRoomCreated", result);
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

        [HttpPost("study-rooms/join")]
        public async Task<IActionResult> JoinStudyRoom([FromBody] JoinStudyRoomRequest request)
        {
            try
            {
                var result = await _gameplayService.JoinStudyRoomAsync(request);
                await _hubContext.Clients.Group($"study-room:{request.RoomId}")
                    .SendAsync("StudyRoomMemberJoined", result);
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

        [HttpPost("study-rooms/leave")]
        public async Task<IActionResult> LeaveStudyRoom([FromBody] LeaveStudyRoomRequest request)
        {
            try
            {
                var result = await _gameplayService.LeaveStudyRoomAsync(request);
                await _hubContext.Clients.Group($"study-room:{request.RoomId}")
                    .SendAsync("StudyRoomMemberLeft", result);
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
    }
}
