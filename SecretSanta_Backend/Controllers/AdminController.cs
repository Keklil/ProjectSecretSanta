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
        public async Task<IActionResult> CreateEvent([FromBody]Event model)
        {
            try
            {
                if (model is null)
                {
                    _logger.LogError("Event object recived from client is null.");
                    return BadRequest("Null object");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Event object recived from client is not valid.");
                    return BadRequest("Invalid object");
                }

                var eventId = Guid.NewGuid();
                var @event = new Event
                {
                    Id = eventId,
                    Description = model.Description,
                    Endofevent = model.Endofevent,
                    Endofregistration = model.Endofregistration
                };

                repository.Event.CreateEvent(@event);
                repository.Save();

                return Ok(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateEvent action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            } 
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEvent([FromBody]Event model)
        {
            return Ok();
        }


        [HttpGet]
        public IEnumerable<Event> Get()
        {
            return repository.Event.FindAll().ToArray();
        }


        [HttpGet("id")]
        public ActionResult<Event> GetById(Guid ID)
        {
            try
            {
                return repository.Event.FindByCondition(x => x.Id == ID).First();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Incorrectly passed ID argument: { ex.Message}.");
                if (ID == Guid.Empty)
                    return BadRequest("Request argument omitted.");
                return BadRequest("Game with this Id does not exist.");
            }
        }
    } 
}