using Microsoft.AspNetCore.Mvc;
using Minitwit_BE.Api.Dtos;
using Minitwit_BE.Domain;
using Minitwit_BE.DomainService.Interfaces;

namespace Minitwit_BE.Api.Controllers
{
    [ApiController]
    [Route("api/twit")]
    public class TwitController : ControllerBase
    {
        private readonly ILogger<TwitController> _logger;
        private readonly IMessageDomainService _messageService;

        public TwitController(ILogger<TwitController> logger, IMessageDomainService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        [HttpGet("test")]
        public async Task<string> TestEndpoint()
        {
            _logger.LogInformation("Test endpoint was called!");

            return await Task.FromResult("test4");
        }


        [HttpPost("add")]
        public async Task<ActionResult> AddTwit([FromBody] MessageDto input)
        {
            ValidateMessageDto(input);

            _logger.LogInformation("Inserting a new twit.");

            var msg = new Message                                       // Primary keys should be auto incremented when you add entity to the table and dont explicitely specify specify the ID
            { 
                AuthorId = input.AuthorId,
                Flagged = false,
                PublishDate = DateTime.Now,
                Text = input.Text
            };

            await _messageService.AddTwit(msg);

            return Ok();
        }

        [HttpGet("public-twits")]
        public async Task<ActionResult<List<Message>>> GetTwits()
        {

            _logger.LogInformation("Returning all public twits");

            var twits = await _messageService.GetTwits();

            return Ok(twits.ToList());
        }

        [HttpGet("personal-twits/{id}")]
        public async Task<ActionResult<List<Message>>> GetPersonalTwits([FromRoute] int id)
        {
            ValidateId(id);
            
            _logger.LogInformation($"Returning all personal twits for user {id}.");

            var twits = await _messageService.GetPersonalTwits(id);

            return Ok(twits.ToList());
        }

        [HttpPut("mark-message")]
        public async Task<ActionResult> MarkMessage([FromBody] FlaggingDto input)
        {
            ValidateFlaggingDto(input);

            _logger.LogInformation($"Mark message endpoint was called on msg id: {input.MessageId}.");

            await _messageService.MarkMessage(input.MessageId, input.FlagMessage);

            return Ok();
        }

        #region PrivateMethods
        private void ValidateFlaggingDto(FlaggingDto input)
        {
            if (input == null || input.MessageId < 0)
            {
                throw new ArgumentException("Input missing or not matching criteria");
            }
        }

        private void ValidateId(int id)
        {
            if (id < 0)
            {
                throw new ArgumentException("Id cannot be negative");
            }
        }

        private void ValidateMessageDto(MessageDto msg)
        {
            if (msg == null || msg.AuthorId < 0 || string.IsNullOrWhiteSpace(msg.Text))
            {
                throw new ArgumentException("Input missing or not matching criteria");
            }

        }
        #endregion
    }
}
