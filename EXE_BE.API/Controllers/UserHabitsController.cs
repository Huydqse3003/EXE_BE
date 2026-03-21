using EXE_BE.Application.DTOs.Requests.UserHabit;
using EXE_BE.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace EXE_BE.API.Controllers
{
    [ApiController]
    [Route("api/user-habits")]
    public class UserHabitsController : ControllerBase
    {
        private readonly IUserHabitService _userHabitService;

        public UserHabitsController(IUserHabitService userHabitService)
        {
            _userHabitService = userHabitService;
        }

        [HttpPost("log")]
        public async Task<IActionResult> LogHabit([FromBody] CreateUserHabitRequest request)
        {
            try
            {
                var result = await _userHabitService.LogHabitAsync(request);
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

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetAllByUser(Guid userId)
        {
            try
            {
                var result = await _userHabitService.GetByUserIdAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{userId:guid}/summary")]
        public async Task<IActionResult> GetSummaryByUser(Guid userId)
        {
            try
            {
                var result = await _userHabitService.GetSummaryByUserIdAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
