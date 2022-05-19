using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SecretSanta_Backend.Models;
using SecretSanta_Backend.ModelsDTO;
using SecretSanta_Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SecretSanta_Backend.Controllers
{
    [ApiController]
    [Route("member")]
    public class MemberController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private ILogger<MemberController> _logger;

        public MemberController(IRepositoryWrapper repository, ILogger<MemberController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("login")]
        public async Task<IActionResult> MemberLogin(MemberLogin login)
        {
            try
            {
                if (login.Email == null)
                {
                    _logger.LogError("Member email recived from client is null.");
                    return BadRequest("Null email");
                }
                if (login.Password == null)
                {
                    _logger.LogError("Member password recived from client is null.");
                    return BadRequest("Null password");
                }

                var member = await _repository.Member.GetMemberByEmailAsync(login.Email);
                if (member == null)
                {
                    _logger.LogInformation("Member recived from client is new.");
                    // TODO:  Auth method to LDAP
                    // check user in LDAP-DB, if exist add email to appDB, if not - auth error
                }

                var memberLogin = new MemberLogin
                {
                    Email = login.Email,
                    Password = login.Password
                };

                // TODO: Auth method here ->
                //return NoContent();
                return StatusCode(200, "{}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMember action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{memberId}/event/{eventId}")]
        public async Task<ActionResult<MemberEventView>> GetEventInfo(Guid memberId, Guid eventId)
        {
            try
            {
                var @event = await _repository.Event.FindByCondition(x => x.Id == eventId).FirstOrDefaultAsync();
                if (@event is null)
                {
                    _logger.LogError("Event object is null.");
                    return BadRequest("Event not found");
                }

                var eventPreferences = await _repository.MemberEvent.FindByCondition(x => x.MemberId == memberId && x.EventId == eventId).FirstOrDefaultAsync();
                var memberAttendCount = await _repository.MemberEvent.FindByCondition(x => x.EventId == eventId).CountAsync();

                MemberEventView view = new MemberEventView
                {
                    Description = @event.Description,
                    EndRegistration = @event.EndRegistration,
                    EndEvent = @event.EndEvent,
                    SumPrice = @event.SumPrice,
                    Preference = eventPreferences.Preference,
                    MembersCount = memberAttendCount
                };

                return Ok(view);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEventInfo action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{memberId}/wishes/{eventId}")]
        public async Task<ActionResult<Wishes>> GetWishes(Guid memberId, Guid eventId)
        {
            try
            {
                var member = await _repository.Member.GetMemberByIdAsync(memberId);
                if (member is null)
                {
                    _logger.LogError("Member object is null.");
                    return BadRequest("Member not found");
                }
                var address = await _repository.Address.FindByCondition(x => x.MemberId == memberId).FirstOrDefaultAsync();
                var preferences = await _repository.MemberEvent.FindByCondition(x => x.MemberId == memberId && x.EventId == eventId).Select(x => x.Preference).FirstOrDefaultAsync();

                Wishes wishes = new Wishes
                {
                    Name = member.Surname + " " + member.Name + " " + member.Patronymic,
                    PhoneNumber = address.PhoneNumber,
                    Zip = address.Zip,
                    Region = address.Region,
                    City = address.City,
                    Street = address.Street,
                    Apartment = address.Apartment,
                    Wish = preferences
                };

                return Ok(wishes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetWishes action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{memberId}/wishes/{eventId}")]
        public async Task<IActionResult> SendWishes(Guid memberId, Guid eventId, [FromBody] Wishes wishes)
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
                    Id = Guid.NewGuid(),
                    MemberId = member.Id,
                    PhoneNumber = wishes.PhoneNumber,
                    Zip = wishes.Zip,
                    Region = wishes.Region,
                    City = wishes.City,
                    Street = wishes.Street,
                    Apartment = wishes.Apartment
                };

                MemberEvent memberEvent = new MemberEvent
                {
                    Id = Guid.NewGuid(),
                    MemberId = memberId,
                    EventId = eventId,
                    MemberAttend = true,
                    Preference = wishes.Wish
                };

                _repository.MemberEvent.CreateMemberEvent(memberEvent);
                _repository.Address.CreateAddress(address);
                _repository.MemberEvent.CreateMemberEvent(memberEvent);
                await _repository.SaveAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SendWishes action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{memberId}/wishes/{eventId}")]
        public async Task<IActionResult> UpdateWishes(Guid memberId, Guid eventId, [FromBody] Wishes wishes)
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
                var address = await _repository.Address.FindByCondition(x => x.MemberId == memberId).FirstOrDefaultAsync();
                var memberEvent = await _repository.MemberEvent.FindByCondition(x => x.MemberId == memberId && x.EventId == eventId).FirstOrDefaultAsync();

                string[] words = wishes.Name.Split(' ');
                member.Surname = words[0];
                member.Name = words[1];
                member.Surname = words[2];

                address.PhoneNumber = wishes.PhoneNumber;
                address.Zip = wishes.Zip;
                address.Region = wishes.Region;
                address.City = wishes.City;
                address.Street = wishes.Street;
                address.Apartment = wishes.Apartment;

                memberEvent.Preference = wishes.Wish;

                _repository.Member.UpdateMember(member);
                _repository.Address.UpdateAddress(address);
                _repository.MemberEvent.UpdateMemberEvent(memberEvent);
                await _repository.SaveAsync();

                //return NoContent();
                return StatusCode(200, "{}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SendWishes action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{memberId}/exit/{eventId}")]
        public async Task<IActionResult> MemberLeaveEvent(Guid memberId, Guid eventId)
        {
            try
            {
                var member = await _repository.MemberEvent.FindByCondition(x => x.MemberId == memberId && x.EventId == eventId).FirstOrDefaultAsync();
                if (member is null)
                {
                    _logger.LogError($"Member object not found");
                    return BadRequest("Member not found");
                }

                member.MemberAttend = false;
                _repository.MemberEvent.UpdateMemberEvent(member);
                await _repository.SaveAsync();

                //return NoContent();
                return StatusCode(200, "{}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside MemberLeaveEvent action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{memberId}/event/{eventId}/gift")]
        public async Task<IActionResult> GetPlaceOfDelivery(Guid memberId, Guid eventId)
        {
            try
            {
                var recipientId = await _repository.MemberEvent.FindByCondition(x => x.MemberId == memberId && x.EventId == eventId).Select(x => x.Recipient).FirstOrDefaultAsync();

                if (recipientId is null)
                {
                    _logger.LogError("Mamber object has not recipient Id.");
                    return BadRequest("No recipient Id");
                }
                else
                {
                    Member recipient = await _repository.Member.GetMemberByIdAsync((Guid)recipientId);
                    var preferences = await _repository.MemberEvent.FindByCondition(x => x.MemberId == (Guid)recipientId && x.EventId == eventId).Select(x => x.Preference).FirstAsync();
                    Address recipientAddress = await _repository.Address.FindByCondition(x => x.MemberId == (Guid)recipientId).FirstAsync();

                    if (recipientAddress.Apartment is null)
                    {
                        GiftFromMe giftFromMe = new GiftFromMe
                        {
                            Name = recipient.Surname + " " + recipient.Name + " " + recipient.Surname,
                            Preferences = preferences,
                            Address = recipientAddress.Zip + ", " + recipientAddress.Region + ", " + recipientAddress.City + ", " + recipientAddress.Street + ", тел. " + recipientAddress.PhoneNumber
                        };
                        return Ok(giftFromMe);
                    }
                    else
                    {
                        GiftFromMe giftFromMe = new GiftFromMe
                        {
                            Name = recipient.Surname + " " + recipient.Name + " " + recipient.Surname,
                            Preferences = preferences,
                            Address = recipientAddress.Zip + ", " + recipientAddress.Region + ", " + recipientAddress.City + ", " + recipientAddress.Street + ", кв. " + recipientAddress.Apartment + ", тел. " + recipientAddress.PhoneNumber
                        };
                        return Ok(giftFromMe);
                    }
                }               
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPlaceOfDelivery action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{memberId}")]
        public async Task<ActionResult<MemberView>> GetMemberById(Guid memberId)
        {
            try 
            {            
                var member = await _repository.Member.GetMemberByIdAsync(memberId);
                if (member is null)
                {
                    _logger.LogError("Member object not found.");
                    return BadRequest("Member not found");
                }

                MemberView memberView = new MemberView
                {
                    Surname = member.Surname,
                    Name = member.Name,
                    Patronymic = member.Patronymic,
                    Email = member.Email
                };
                return Ok(memberView); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMemberById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
