using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanoAssistant.Core.GrainInterfaces;
using NanoAssistant.Shared.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;


namespace NanoAssistant.BlazorWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromServices] IHttpContextAccessor httpContextAccessor, [FromServices]IClusterClient clusterClient, [FromBody] UserMessageDto userMessage)
        {

            string? token = await httpContextAccessor?.HttpContext?.GetTokenAsync("access_token");
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized();
            }
            string? userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var assistant = clusterClient.GetGrain<INanoAssistantGrain>(userId);

            var chatDto = await assistant.AddUserMessage(userMessage, token);
            return Ok(chatDto);
        }

        private string? GetUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        [Authorize()]
        [HttpGet("history")]
        public async Task<IActionResult> Get([FromServices] IClusterClient clusterClient)
        {

            string? userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            var assistant = clusterClient.GetGrain<INanoAssistantGrain>(userId);
            var chatHistory = await assistant.GetChatHistoryAsync();
            return Ok(chatHistory);
        }
    }
}
