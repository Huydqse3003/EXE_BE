using EXE_BE.Application.DTOs.Requests.FocusMusic;
using EXE_BE.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace EXE_BE.API.Controllers
{
    [ApiController]
    [Route("api/focus-music")]
    public class FocusMusicController : ControllerBase
    {
        private readonly IFocusMusicService _focusMusicService;

        public FocusMusicController(IFocusMusicService focusMusicService)
        {
            _focusMusicService = focusMusicService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateFocusMusicRequest request)
        {
            try
            {
                var result = await _focusMusicService.AddAsync(request);
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

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            try
            {
                var result = await _focusMusicService.GetByUserIdAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user/{userId:guid}/current")]
        public async Task<IActionResult> GetCurrent(Guid userId)
        {
            try
            {
                var result = await _focusMusicService.GetCurrentByUserIdAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("user/{userId:guid}/current/{musicId:guid}")]
        public async Task<IActionResult> SetCurrent(Guid userId, Guid musicId)
        {
            try
            {
                var result = await _focusMusicService.SetCurrentAsync(userId, musicId);
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
