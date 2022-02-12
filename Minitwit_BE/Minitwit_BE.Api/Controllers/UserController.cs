using Microsoft.AspNetCore.Mvc;
using Minitwit_BE.Domain;
using Minitwit_BE.Persistence;

namespace Minitwit_BE.Api.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly TwitContext _twitContext;
        private readonly ILogger<UserController> _logger;

        public UserController(TwitContext twitContext, ILogger<UserController> logger)
        {
            _twitContext = twitContext;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser([FromBody]UserInput input)
        {
            _logger.LogInformation($"RegisterUser endpoint was called for user: {input.UserName}");

            var existingUser = _twitContext.Users.FirstOrDefault(user => user.UserName.Equals(input.UserName) || user.Email.Equals(input.Email));
            
            if (existingUser != null) 
            {
                return BadRequest("User with that nickname or email already exists!");
            }

            var user = new User
            {
                UserName = input.UserName,
                Email = input.Email,
                PwHash = input.PwHash,
            };

            _twitContext.Add(user);
            _twitContext.SaveChanges();

            return Ok(user);
        }

        [HttpGet("login")]
        public async Task<ActionResult<int>> Login([FromBody]UserInput input)
        {
            _logger.LogInformation($"Login endpoint was called for user: {input.Email}");

            var authUser = _twitContext.Users.FirstOrDefault(user => user.Email.Equals(input.Email) && user.PwHash.Equals(input.PwHash));

            return authUser != null ? Ok(authUser.UserId) : Unauthorized();
        }
    }
}