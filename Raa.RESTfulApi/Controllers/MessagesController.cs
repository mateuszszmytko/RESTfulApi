using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Raa.AspNetCore.MongoDataContext.Repository;
using Raa.RESTfulApi.Entities;
using Raa.RESTfulApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raa.RESTfulApi.Controllers
{
    [Authorize]
    [Route("api/messages")]
    public class MessagesController : Controller
    {
        private Repository<UserMessage> _messages;
        private UserManager<ApplicationUser> _userManager;

        public MessagesController(Repository<UserMessage> messages, UserManager<ApplicationUser> userManager)
        {
            _messages = messages;
            _userManager = userManager;
        }

        [HttpGet("")]
        public IActionResult GetMessages()
        {
            var messages = _messages.List;
            var users = _userManager.Users;

            var messagesDto = new List<ReturnMessageDto>();

            foreach(var message in messages)
            {
                var messageDto = new ReturnMessageDto
                {
                    Message = message.Message,
                    User = users.FirstOrDefault(u => u.Id == message.UserId).UserName
                };

                messagesDto.Add(messageDto);
            }
            return Ok(messagesDto);
        }

        [HttpGet("user/{userId}", Name = "GetUserMessages")]
        public async Task<IActionResult> GetUserMessages(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest();

            var messages = _messages.List.Where(u => u.Id == user.Id);
            if (messages.Count() == 0) return NotFound();

            var messagesDto = new List<ReturnMessageDto>();

            foreach (var message in messages)
            {
                var messageDto = new ReturnMessageDto
                {
                    Message = message.Message,
                    User = user.UserName
                };

                messagesDto.Add(messageDto);
            }

            return Ok(messagesDto);
        }



        [HttpGet("user")]
        public async Task<IActionResult> GetUserMessages()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return BadRequest();

            var messages = _messages.List.Where(m => m.UserId == user.Id);
            if (messages.Count() == 0) return NotFound();

            var messagesDto = new List<ReturnMessageDto>();

            foreach (var message in messages)
            {
                var messageDto = new ReturnMessageDto
                {
                    Message = message.Message,
                    User = user.UserName
                };

                messagesDto.Add(messageDto);
            }

            return Ok(messagesDto);
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(string id)
        {
            var message = _messages.List.First(m => m.Id.ToString() == id);
            if (message == null) return NotFound();

            var user = await _userManager.FindByIdAsync(message.UserId.ToString());
            if (user == null) return NotFound();

            var messageDto = new ReturnMessageDto
            {
                Message = message.Message,
                User = user.UserName
            };

            return Ok(messageDto);
        }


        [HttpPost()]
        public async Task<IActionResult> AddMessage([FromBody] CreateMessageDto message)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return BadRequest();

            var messageFromRepo = new UserMessage
            {
                //Id = ObjectId.GenerateNewId(),
                Message = message.Message,
                UserId = user.Id
            };

            await _messages.InsertAsync(messageFromRepo);

            var messageDto = new ReturnMessageDto
            {
                Message = messageFromRepo.Message,
                User = user.UserName
            };

            return CreatedAtRoute("GetMessage", new { id = messageFromRepo.Id }, messageDto);
        }


    }
}
