using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NanoAssistant.Core.Dtos;
using NanoAssistant.Core.GrainInterfaces;

namespace NanoAssistant.BlazorWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssistantController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices]IClusterClient clusterClient, [FromBody] UserMessage userMessage)
        {
            var assistant = clusterClient.GetGrain<INanoAssistantGrain>("user-1");
            var response = assistant.AddUserMessage(userMessage);
            return Ok(new { response = response.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] IClusterClient clusterClient, [FromQuery] string message)
        {
            var assistant = clusterClient.GetGrain<INanoAssistantGrain>("user-1");
            var response = await assistant.AddUserMessage(new UserMessage() { Message = message });
            return Ok(new { response = response.ToString() });
        }
    }
}
