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
    [Route("user")]
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
                    return BadRequest(new { message = "Null email" });
                }
                if (login.Password == null)
                {
                    _logger.LogError("Member password recived from client is null.");
                    return BadRequest(new { message = "Null password" });
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
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{userId}/event/{eventId}")]
        public async Task<ActionResult<MemberEventView>> GetEventInfo(Guid userId, Guid eventId)
        {
            try
            {
                var @event = await _repository.Event.FindByCondition(x => x.Id == eventId).FirstOrDefaultAsync();
                if (@event is null)
                {
                    _logger.LogError("Event object is null.");
                    return BadRequest(new { message = "Event not found" });
                }

                var eventPreferences = await _repository.MemberEvent.FindByCondition(x => x.MemberId == userId && x.EventId == eventId).FirstOrDefaultAsync();
                var memberAttendCount = await _repository.MemberEvent.FindByCondition(x => x.EventId == eventId).CountAsync();

                MemberEventView view = new MemberEventView
                {
                    Description = @event.Description,
                    EndRegistration = @event.EndRegistration,
                    EndEvent = @event.EndEvent,
                    SumPrice = @event.SumPrice,
                    Preference = eventPreferences.Preference,
                    MembersCount = memberAttendCount,
                    Reshuffle = @event.Reshuffle
                };

                return Ok(view);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEventInfo action: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{userId}/preferences/{eventId}")]
        public async Task<ActionResult<Preferences>> GetPreferences(Guid userId, Guid eventId)
        {
            try
            {
                var member = await _repository.Member.GetMemberByIdAsync(userId);
                if (member is null)
                {
                    _logger.LogError("Member object is null.");
                    return BadRequest(new { message = "Member not found" });
                }
                var address = await _repository.Address.FindByCondition(x => x.MemberId == userId).FirstOrDefaultAsync();
                var preferences = await _repository.MemberEvent.FindByCondition(x => x.MemberId == userId && x.EventId == eventId).Select(x => x.Preference).FirstOrDefaultAsync();

                Preferences wishes = new Preferences
                {
                    Name = member.Surname + " " + member.Name + " " + member.Patronymic,
                    PhoneNumber = address.PhoneNumber,
                    Zip = address.Zip,
                    Region = address.Region,
                    City = address.City,
                    Street = address.Street,
                    Apartment = address.Apartment,
                    Preference = preferences
                };

                return Ok(wishes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetWishes action: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("{userId}/preferences/{eventId}")]
        public async Task<IActionResult> SendPreferences(Guid userId, Guid eventId, [FromBody] Preferences preferences)
        {
            try
            {
                if (preferences is null)
                {
                    _logger.LogError("Preferences object recived from client is null.");
                    return BadRequest(new { message = "Null object" });
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Preferences object recived from client is not valid.");
                    return BadRequest(new { message = "Invalid object"});
                }
                var @event = await _repository.Event.FindByCondition(x => x.Id == eventId).FirstOrDefaultAsync();
                if (@event.Reshuffle == true)
                {
                    _logger.LogError("Registration date has already expired");
                    return BadRequest(new { message = "Registration date has already expired" });
                }

                Member member = await _repository.Member.GetMemberByIdAsync(userId);

                if (member.Name is null || member.Surname is null || member.Patronymic is null)
                {
                    string[] words = preferences.Name.Split(' ');

                    member.Surname = words[0];
                    member.Name = words[1];
                    member.Surname = words[2];
                    _repository.Member.UpdateMember(member);
                }

                Address address = new Address
                {
                    Id = Guid.NewGuid(),
                    MemberId = member.Id,
                    PhoneNumber = preferences.PhoneNumber,
                    Zip = preferences.Zip,
                    Region = preferences.Region,
                    City = preferences.City,
                    Street = preferences.Street,
                    Apartment = preferences.Apartment
                };

                MemberEvent memberEvent = new MemberEvent
                {
                    Id = Guid.NewGuid(),
                    MemberId = userId,
                    EventId = eventId,
                    MemberAttend = true,
                    Preference = preferences.Preference
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
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{userId}/preferences/{eventId}")]
        public async Task<IActionResult> UpdateWishes(Guid userId, Guid eventId, [FromBody] Preferences preferences)
        {
            try
            {
                if (preferences is null)
                {
                    _logger.LogError("Wishes object recived from client is null.");
                    return BadRequest(new { message = "Null object" });
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Wishes object recived from client is not valid.");
                    return BadRequest(new { message = "Invalid object" });
                }
                var @event = await _repository.Event.FindByCondition(x => x.Id == eventId).FirstOrDefaultAsync();
                if (@event.Reshuffle == true)
                {
                    _logger.LogError("Registration date has already expired");
                    return BadRequest(new { message = "Registration date has already expired" });
                }

                Member member = await _repository.Member.GetMemberByIdAsync(userId);
                var address = await _repository.Address.FindByCondition(x => x.MemberId == userId).FirstOrDefaultAsync();
                var memberEvent = await _repository.MemberEvent.FindByCondition(x => x.MemberId == userId && x.EventId == eventId).FirstOrDefaultAsync();

                string[] words = preferences.Name.Split(' ');
                member.Surname = words[0];
                member.Name = words[1];
                member.Surname = words[2];

                address.PhoneNumber = preferences.PhoneNumber;
                address.Zip = preferences.Zip;
                address.Region = preferences.Region;
                address.City = preferences.City;
                address.Street = preferences.Street;
                address.Apartment = preferences.Apartment;

                memberEvent.Preference = preferences.Preference;

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
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{userId}/exit/{eventId}")]
        public async Task<IActionResult> MemberLeaveEvent(Guid userId, Guid eventId)
        {
            try
            {
                var @event = await _repository.Event.FindByCondition(x => x.Id == eventId).FirstOrDefaultAsync();
                if (@event.Reshuffle == true)
                {
                    _logger.LogError("Registration date has already expired");
                    return BadRequest(new { message = "Registration date has already expired" });
                }
                var member = await _repository.MemberEvent.FindByCondition(x => x.MemberId == userId && x.EventId == eventId).FirstOrDefaultAsync();
                if (member is null)
                {
                    _logger.LogError($"Member object not found");
                    return BadRequest(new { message = "Member not found" });
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
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{userId}/event/{eventId}/recipientInfo")]
        public async Task<ActionResult<GiftFromMe>> GetPlaceOfDelivery(Guid userId, Guid eventId)
        {
            try
            {
                var recipientId = await _repository.MemberEvent.FindByCondition(x => x.MemberId == userId && x.EventId == eventId).Select(x => x.Recipient).FirstOrDefaultAsync();

                if (recipientId is null)
                {
                    _logger.LogError("Mamber object has not recipient Id.");
                    return BadRequest(new { message = "No recipient Id" });
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
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<MemberView>> GetMemberById(Guid userId)
        {
            try 
            {            
                var member = await _repository.Member.GetMemberByIdAsync(userId);
                if (member is null)
                {
                    _logger.LogError("Member object not found.");
                    return BadRequest(new { message = "Member not found" });
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
                return StatusCode(500, new { message = "Internal server error" });
            }

        }
    }
}
