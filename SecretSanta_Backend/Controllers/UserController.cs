using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SecretSanta_Backend.Models;
using SecretSanta_Backend.Interfaces;
using AutoMapper;

namespace SecretSanta_Backend.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IRepositoryWrapper _repository;
        private ILogger<UserController> _logger;

        public UserController(IMapper mapper, IRepositoryWrapper repository, ILogger<UserController> logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }


    }
}
