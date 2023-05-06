using ChatGptDemo.Dtos;
using ChatGptDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatGptDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageDTO request)
        {
            var response = await _chatService.SendMessage(request.Message, request.ChatHistory);

            return Ok(response.Choices[0].Message);

        }
    }
}
