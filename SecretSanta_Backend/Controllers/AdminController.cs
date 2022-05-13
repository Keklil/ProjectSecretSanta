using Microsoft.AspNetCore.Mvc;
using SecretSanta_Backend.Models;
using SecretSanta_Backend.ModelsDTO;
using SecretSanta_Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SecretSanta_Backend.Controllers
{
    [ApiController]
    [Route("event")]

    public class AdminController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger, IRepositoryWrapper repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventCreate @event)
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

                var eventId = Guid.NewGuid();
                var eventResult = new Event
                {
                    Id = eventId,
                    Description = @event.Description,
                    EndEvent = @event.EndEvent,
                    EndRegistration = @event.EndRegistration,
                    SumPrice = @event.Sumprice,
                    SendFriends = @event.Sendfriends,
                    Tracking = @event.Tracking
                };

                _repository.Event.CreateEvent(eventResult);
                await _repository.SaveAsync();

                return Ok(eventResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateEvent action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEvent(Guid eventId)
        {
            try
            {
                var @event = await _repository.Event.FindByCondition(x => x.Id == eventId).SingleAsync();

                if (@event is null)
                {
                    _logger.LogError($"Event with ID: {eventId} not found");
                    return BadRequest("Event not found");
                }

                _repository.Event.DeleteEvent(@event);
                await _repository.SaveAsync();

                return Ok(null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Incorrectly passed ID argument: { ex.Message}.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("events")]
        public async Task<IEnumerable<Event>> GetEvents()
        {
            return await _repository.Event.FindAll().ToListAsync();
        }


        [HttpGet("{eventId}")]
        public async Task<ActionResult<Event>> GetEventById(Guid eventId)
        {
            try
            {   if (eventId == Guid.Empty)
                    return BadRequest("Request argument omitted.");                
                var @event =  await _repository.Event.FindByCondition(x => x.Id == eventId).FirstAsync();
                if (@event is null)
                    return BadRequest("Game with this Id does not exist.");
                return @event;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Incorrectly passed ID argument: { ex.Message}.");
                return StatusCode(500, "Internal server error");
                
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEvent([FromBody]Event @event)
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

                _repository.Event.UpdateEvent(@event);
                await _repository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Incorrectly passed argument: { ex.Message}.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("events/{memberId}")]
        public async Task<IActionResult> GetEventsByMember(Guid memberId)
        {
            try
            {
                var events = _repository.Event.GetEventsByMemberId(memberId);

                if (events == null)
                {
                    _logger.LogError("Member is not take part one more event");
                    return BadRequest("Events null");
                }

                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEventsList action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    } 
}