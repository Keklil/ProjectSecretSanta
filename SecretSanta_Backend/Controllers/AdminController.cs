using Microsoft.AspNetCore.Mvc;
using SecretSanta_Backend.Services;
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
                    EndEvent = @event.EndEvent.SetKindUtc(),
                    EndRegistration = @event.EndRegistration.SetKindUtc(),
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
                var @event = await _repository.Event.FindByCondition(x => x.Id == eventId).FirstOrDefaultAsync();

                if (@event is null)
                {
                    _logger.LogError($"Event with ID: {eventId} not found");
                    return BadRequest("Event not found");
                }

                _repository.Event.DeleteEvent(@event);
                await _repository.SaveAsync();

                //return NoContent();
                return StatusCode(200, "{}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Incorrectly passed ID argument: { ex.Message}.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("events")]
        public async Task<ActionResult<EventView>> GetEvents()
        {
            var events = await _repository.Event.FindAll().ToListAsync();
            if (events is null)
                return BadRequest("Events does not exist.");

            List<EventView> eventsList = new List<EventView>();

            foreach (var @event in events)
            {
                EventView view = new EventView
                {
                    Id = @event.Id,
                    Description = @event.Description,
                    EndRegistration = @event.EndRegistration,
                    EndEvent = @event.EndEvent,
                    SumPrice = @event.SumPrice,
                    Tracking = @event.Tracking
                };
                eventsList.Add(view);
            }

            return Ok(eventsList);
        }


        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventView>> GetEventById(Guid eventId)
        {
            try
            { 
                if (eventId == Guid.Empty)
                    return BadRequest("Request argument omitted.");
                var @event = await _repository.Event.FindByCondition(x => x.Id == eventId).FirstOrDefaultAsync();
                if (@event is null)
                    return BadRequest("Game with this Id does not exist.");

                EventView eventView = new EventView
                {
                    Id = eventId,
                    Description = @event.Description,
                    EndRegistration = @event.EndRegistration,
                    EndEvent = @event.EndEvent,
                    SumPrice = @event.SumPrice,
                    Tracking = @event.Tracking
                };

                return Ok(eventView);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Incorrectly passed ID argument: { ex.Message}.");
                return StatusCode(500, "Internal server error");

            }
        }

        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateEventById(Guid eventId, [FromBody]EventCreate @event)
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

                var eventResult = await _repository.Event.FindByCondition(x => x.Id == eventId).FirstOrDefaultAsync();
                if (eventResult is null)
                {
                    _logger.LogError("Event object not found.");
                    return BadRequest("Event not found");
                }

                eventResult.Description = @event.Description;
                eventResult.EndRegistration = @event.EndRegistration.SetKindUtc();
                eventResult.EndEvent = @event.EndEvent.SetKindUtc();
                eventResult.SumPrice = @event.Sumprice;
                eventResult.Tracking = @event.Tracking;

                _repository.Event.UpdateEvent(eventResult);
                await _repository.SaveAsync();

                //return NoContent();
                return StatusCode(200, "{}");
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
                    return BadRequest("Events not found");
                }

                List<EventView> eventsList = new List<EventView>();

                foreach (var @event in events)
                {
                    EventView view = new EventView
                    {
                        Id = @event.Id,
                        Description = @event.Description,
                        EndRegistration = @event.EndRegistration,
                        EndEvent = @event.EndEvent,
                        SumPrice = @event.SumPrice,
                        Tracking = @event.Tracking
                    };
                    eventsList.Add(view);
                }

                return Ok(eventsList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEventsList action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    } 
}