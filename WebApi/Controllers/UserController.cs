using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebApi.RabbitMQ;
using WebApi.Redis;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly static IList<User> Users = new List<User>()
        {
            new User { Id = 1, Name = "Alex", Position = "OOS" },
            new User { Id = 2, Name = "Hagi", Position = "RW" },
            new User { Id = 3, Name = "Maradona", Position = "RW" },
            new User { Id = 4, Name = "Ronaldo", Position = "SF" },
            new User { Id = 5, Name = "Messi", Position = "OOS" },
            new User { Id = 6, Name = "Xavi", Position = "GO" },
            new User { Id = 7, Name = "Iniesta", Position = "GO" },
            new User { Id = 8, Name = "Maldini", Position = "CF" },
        };

        private readonly ILogger<UserController> _logger;
        private readonly RabbitMqService _rabbitMqService;
        private readonly ICacheManager _cacheManager;

        public UserController(ILogger<UserController> logger, ICacheManager cacheManager, RabbitMqService rabbitMqService)
        {
            _logger = logger;
            _cacheManager = cacheManager;
            _rabbitMqService = rabbitMqService;
        }
        [HttpPost]
        public IActionResult Add(User user)
        {
            user.DateTime = DateTime.Now;
            Users.Add(user);

            _rabbitMqService.Publish("", "user", JsonConvert.SerializeObject(user));
            _logger.LogDebug("Message published to queue");

            _cacheManager.Add("users", Users, 100);
            _logger.LogDebug("Users added to cache");

            return Ok("Process successfully");
        }
        [HttpGet]
        public IEnumerable<User> Get()
        {
            var users = _cacheManager.Get<List<User>>("users");
            return users == null ? Users : users;
        }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public DateTime DateTime { get; set; }
    }
}
