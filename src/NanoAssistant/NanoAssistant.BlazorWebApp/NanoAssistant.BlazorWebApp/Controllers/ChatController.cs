using Microsoft.AspNetCore.Mvc;
using NanoAssistant.Core.GrainInterfaces;
using NanoAssistant.Shared.Dtos;

namespace NanoAssistant.BlazorWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices]IClusterClient clusterClient, [FromBody] UserMessageDto userMessage)
        {
            var assistant = clusterClient.GetGrain<INanoAssistantGrain>("user-1");
            var chatDto = await assistant.AddUserMessage(userMessage);
            return Ok(chatDto);
        }

        [HttpGet("history")]
        public async Task<IActionResult> Get([FromServices] IClusterClient clusterClient)
        {
            var assistant = clusterClient.GetGrain<INanoAssistantGrain>("user-1");
            var chatHistory = await assistant.GetChatHistoryAsync();
            return Ok(chatHistory);
        }
    }
}
