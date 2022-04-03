using Microsoft.AspNetCore.Mvc;
using SecretSanta_Backend.Models;
using SecretSanta_Backend.Interfaces;

namespace SecretSanta_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private IRepositoryWrapper repository;

        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger, IRepositoryWrapper repository)
        {
            _logger = logger;
            this.repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(Event @event)
        {
            try
            {
                if (@event is null)
                {
                    _logger.LogError("Event object recived from client is null.");
                    return BadRequest("Null object");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Event object recived from client is not valid.");
                    return BadRequest("Invalid object");
                }

                repository.Event.CreateEvent(@event);
                repository.Save();

                return Ok(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            } 
        }
    }
}