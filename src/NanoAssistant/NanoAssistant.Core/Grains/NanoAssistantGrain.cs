using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using NanoAssistant.Core.GrainInterfaces;
using NanoAssistant.Shared.Dtos;

namespace NanoAssistant.Core.Grains
{
    public class NanoAssistantGrain : Grain<NanoAssistantState>, INanoAssistantGrain
    {
        private readonly IChatCompletionService _chatCompletionService;
        private readonly Kernel _kernel;
        private readonly PromptExecutionSettings _promptExecutionSettings;

        public NanoAssistantGrain(IChatCompletionService chatCompletionService, Kernel kernel, PromptExecutionSettings promptExecutionSettings)
        {
            _chatCompletionService = chatCompletionService;
            _kernel = kernel;
            _promptExecutionSettings = promptExecutionSettings;
        }

        public async Task<ChatDto> AddUserMessage(UserMessageDto userMessage)
        {
            State.ChatHistory.AddUserMessage(userMessage.Message);
            var result = await _chatCompletionService.GetChatMessageContentAsync(State.ChatHistory, _promptExecutionSettings, _kernel);
            State.ChatHistory.Add(result);
            await WriteStateAsync();
            return new ChatDto
            {
                Role = result.Role.ToString(),
                Message = result.ToString(),
            };
        }

        public async Task<ChatHistoryDto> GetChatHistoryAsync()
        {
            return new ChatHistoryDto()
            {
                Chats = State.ChatHistory.Select(chat => new ChatDto()
                {
                    Role = chat.Role.ToString(),
                    Message = chat.ToString(),
                }).ToList()
            };
        }
    }

    public class NanoAssistantState
    {
        public ChatHistory ChatHistory { get; set; } = [];
    }
}
