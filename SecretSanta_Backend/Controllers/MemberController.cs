using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SecretSanta_Backend.Models;
using SecretSanta_Backend.ModelsDTO;
using SecretSanta_Backend.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SecretSanta_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IRepositoryWrapper _repository;
        private ILogger<MemberController> _logger;

        public MemberController(IMapper mapper, IRepositoryWrapper repository, ILogger<MemberController> logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateMember([FromBody] Member member)
        {
            try
            {
                if (member is null)
                {
                    _logger.LogError("Member object recived from client is null.");
                    return BadRequest("Null object");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Member object recived from client is not valid.");
                    return BadRequest("Invalid object");
                }

                var memberId = Guid.NewGuid();
                var memberResult = new Member
                {
                    Id = memberId
                };

                _repository.Member.CreateMember(memberResult);
                _repository.Save();

                return Ok(memberResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMember action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("login")]
        public async Task<IActionResult> MemberLogin([FromBody] string email, string password)
        {
            try
            {
                if (email == null)
                {
                    _logger.LogError("Member email recived from client is null.");
                    return BadRequest("Null email");
                }
                if (password == null)
                {
                    _logger.LogError("Member password recived from client is null.");
                    return BadRequest("Null password");
                }

                var member = await _repository.Member.GetMemberByEmailAsync(email);
                if (member == null)
                {
                    _logger.LogInformation("Member recived from client is not found.");
                    // TODO:  Auth method to LDAP
                    // check user in LDAP-DB, if exist add email to appDB, if not - auth error
                }

                var memberLogin = new MemberLogin
                {
                    Email = email,
                    Password = password
                };

                // TODO: Auth method here ->
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMember action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/wishes/{eventId}")]
        public async Task<IActionResult> GetWishes(Guid memberId, Guid eventId)
        {
            try
            {
                Member member = await _repository.Member.GetMemberByIdAsync(memberId);
                Address address = await _repository.Address.FindByCondition(x => x.MemberId == memberId).FirstAsync();
                MemberEvent memberEvent = await _repository.MemberEvent.FindByCondition(x => x.MemberId == memberId && x.EventId == eventId).FirstAsync();

                Wishes wishes = new Wishes
                {
                    Name = member.Surname + " " + member.Name + " " + member.Patronymic,
                    PhoneNumber = address.PhoneNumber,
                    Zip = address.Zip,
                    Region = address.Region,
                    City = address.City,
                    Street = address.Street,
                    Apartment = address.Apartment,
                    Wish = memberEvent.Preference
                };

                return Ok(wishes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetWishes action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/wishes")]
        public async Task<IActionResult> SendWishes(Guid memberId,[FromBody] Wishes wishes)
        {
            try
            {
                if (wishes is null)
                {
                    _logger.LogError("Wishes object recived from client is null.");
                    return BadRequest("Null object");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Wishes object recived from client is not valid.");
                    return BadRequest("Invalid object");
                }

                Member member = await _repository.Member.GetMemberByIdAsync(memberId);

                if (member.Name is null || member.Surname is null || member.Patronymic is null)
                {
                    string[] words = wishes.Name.Split(' ');

                    member.Surname = words[0];
                    member.Name = words[1];
                    member.Surname = words[2];
                    _repository.Member.UpdateMember(member);
                }

                Address address = new Address
                {
                    MemberId = member.Id,
                    PhoneNumber = wishes.PhoneNumber,
                    Zip = wishes.Zip,
                    Region = wishes.Region,
                    City = wishes.City,
                    Street = wishes.Street,
                    Apartment = wishes.Apartment,
                    Member = member
                };

                _repository.Address.CreateAddress(address);
                await _repository.SaveAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SendWishes action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("/{id}/event/{eventId}")]
        public async Task<IActionResult> GetEventInfo(Guid id, Guid eventId)
        {
            var @event = await _repository.Event.FindByCondition(x => x.Id == eventId).SingleAsync();
            var eventPreferences = await _repository.MemberEvent.FindByCondition(x => x.MemberId == id && x.EventId == eventId).SingleAsync();
            var memberAttendCount = _repository.MemberEvent.FindByCondition(x => x.MemberId == id).Count();

            UserEventView view = new UserEventView
            {
                Description = @event.Description,
                EndRegistration = @event.EndRegistration,
                EndEvent = @event.EndEvent,
                SumPrice = @event.SumPrice,
                UsersCount = memberAttendCount
            };

            return Ok(view);
        }

        [HttpPut("exit")]
        public async Task<IActionResult> MemberLeaveEvent([FromBody] Guid memberId, Guid eventId)
        {
            var member = _repository.MemberEvent.FindByCondition(x => x.MemberId == memberId && x.EventId == eventId).First();
            member.MemberAttend = false;
            _repository.MemberEvent.Update(member);
            await _repository.SaveAsync();

            return NoContent();
        }

    }
}
