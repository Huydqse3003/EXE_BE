using EXE_BE.Application.DTOs.Requests.FocusMusic;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_BE.API.Controllers
{
    [ApiController]
    [Route("api/focus-music")]
    [Authorize]
    public class FocusMusicController : ControllerBase
    {
        private readonly IFocusMusicService _focusMusicService;

        public FocusMusicController(IFocusMusicService focusMusicService)
        {
            _focusMusicService = focusMusicService;
        }

        /// <summary>
        /// Thêm bài nhạc tập trung cho người dùng.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateFocusMusicRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse().SetBadRequest(message: string.Join("; ", errors)));
            }

            var response = await _focusMusicService.AddAsync(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Lấy danh sách nhạc tập trung của một người dùng.
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var response = await _focusMusicService.GetByUserIdAsync(userId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Lấy bài nhạc hiện tại đang được chọn của người dùng.
        /// </summary>
        [HttpGet("user/{userId:guid}/current")]
        public async Task<IActionResult> GetCurrent(Guid userId)
        {
            var response = await _focusMusicService.GetCurrentByUserIdAsync(userId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Đặt một bài nhạc làm nhạc hiện tại cho người dùng.
        /// </summary>
        [HttpPost("user/{userId:guid}/current/{musicId:guid}")]
        public async Task<IActionResult> SetCurrent(Guid userId, Guid musicId)
        {
            var response = await _focusMusicService.SetCurrentAsync(userId, musicId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
