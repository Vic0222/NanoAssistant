using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanoAssistant.Core.Dtos;
using NanoAssistant.Core.GrainInterfaces;

namespace NanoAssistant.BlazorWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices]IClusterClient clusterClient, [FromBody] UserMessage userMessage)
        {
            var assistant = clusterClient.GetGrain<INanoAssistantGrain>("user-1");
            var response = assistant.AddUserMessage(userMessage);
            return Ok(new { response = response.ToString() });
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
