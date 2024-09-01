using NanoAssistant.Shared.Dtos;

namespace NanoAssistant.Core.GrainInterfaces
{
    public interface INanoAssistantGrain : IGrainWithStringKey
    {
        Task<ChatDto> AddUserMessage(UserMessageDto userMessage, string accessToken);
        Task<ChatHistoryDto> GetChatHistoryAsync();
    }
}
