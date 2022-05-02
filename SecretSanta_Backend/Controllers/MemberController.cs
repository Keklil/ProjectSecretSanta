using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SecretSanta_Backend.Models;
using SecretSanta_Backend.ModelsDTO;
using SecretSanta_Backend.Interfaces;
using AutoMapper;

namespace SecretSanta_Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
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

        [HttpPost]
        public async Task<IActionResult> MemberLogin(string email, string password)
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

                var memberLogin = new MemberLogin
                {
                    Email = email,
                    Password = password
                };

                var member = await _repository.Member.GetMemberByEmailAsync(email);
                if(member == null)
                {
                    _logger.LogError("Member recived from client is nor found.");
                    return BadRequest("Member not found");
                }
                // TODO: Auth method here ->
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMember action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
