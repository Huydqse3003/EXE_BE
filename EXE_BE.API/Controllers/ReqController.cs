using EXE_BE.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_BE.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public HealthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                success = true,
                message = "pong",
                utcTime = DateTime.UtcNow
            });
        }

        [AllowAnonymous]
        [HttpHead("ping")]
        public IActionResult PingHead()
        {
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("db")]
        public async Task<IActionResult> Db()
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();

                return Ok(new
                {
                    success = canConnect,
                    database = canConnect ? "reachable" : "unreachable",
                    utcTime = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database check failed",
                    error = ex.Message,
                    utcTime = DateTime.UtcNow
                });
            }
        }

        [AllowAnonymous]
        [HttpHead("db")]
        public IActionResult DbHead()
        {
            return Ok();
        }
    }
}