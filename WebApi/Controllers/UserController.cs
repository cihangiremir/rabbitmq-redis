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
        private readonly ILogger<UserController> _logger;
        static IList<User> Users = new List<User>()
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
        private Publisher _publisher;
        private Consumer _consumer;
        private ICacheManager _cacheManager;

        public UserController(ILogger<UserController> logger, ICacheManager cacheManager)
        {
            _logger = logger;
            _cacheManager = cacheManager;
        }
        [HttpPost]
        public IActionResult Add(User user)
        {
            user.DateTime = DateTime.Now;
            _publisher = new Publisher("user", JsonConvert.SerializeObject(user));
            Users.Add(user);
            _cacheManager.Add("users", Users, 100);
            return Ok("Kullanıcı Kuyruğa ve Cache'e Eklendi..");
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
