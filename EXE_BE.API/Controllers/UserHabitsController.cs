using EXE_BE.Application.DTOs.Requests.UserHabit;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_BE.API.Controllers
{
    [ApiController]
    [Route("api/user-habits")]
    [Authorize]
    public class UserHabitsController : ControllerBase
    {
        private readonly IUserHabitService _userHabitService;

        public UserHabitsController(IUserHabitService userHabitService)
        {
            _userHabitService = userHabitService;
        }

        /// <summary>
        /// Ghi nhận một hoạt động thói quen của người dùng.
        /// </summary>
        [HttpPost("log")]
        public async Task<IActionResult> LogHabit([FromBody] CreateUserHabitRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse().SetBadRequest(message: string.Join("; ", errors)));
            }

            var response = await _userHabitService.LogHabitAsync(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Lấy toàn bộ lịch sử thói quen theo người dùng.
        /// </summary>
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetAllByUser(Guid userId)
        {
            var response = await _userHabitService.GetByUserIdAsync(userId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Lấy dữ liệu tổng hợp thói quen của người dùng.
        /// </summary>
        [HttpGet("{userId:guid}/summary")]
        public async Task<IActionResult> GetSummaryByUser(Guid userId)
        {
            var response = await _userHabitService.GetSummaryByUserIdAsync(userId);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}
