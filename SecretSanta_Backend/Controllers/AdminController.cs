using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SecretSanta_Backend.Models;
using SecretSanta_Backend.Interfaces;
using AutoMapper;

namespace SecretSanta_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private IRepositoryWrapper repository;
        private IMapper mapper;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            this.mapper = mapper;
            this.repository = repository;
        }

        [HttpPost]
        public IActionResult CreateEvent([FromBody]Event model)
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
                    EndEvent = model.EndEvent,
                    EndRegistration = model.EndRegistration
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

        [HttpDelete("id")]
        public IActionResult DeleteEvent(Guid ID)
        {
            try
            {
                var @event = repository.Event.FindByCondition(x => x.Id == ID).First();
                if (@event is null)
                {
                    _logger.LogError($"Event with ID: {ID} not found");
                    return BadRequest("Event not found");
                }

                repository.Event.Delete(@event);
                repository.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Incorrectly passed ID argument: { ex.Message}.");
                return StatusCode(500, "Internal server error");
            }
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