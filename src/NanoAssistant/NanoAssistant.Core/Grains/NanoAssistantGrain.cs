using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using NanoAssistant.Core.GrainInterfaces;
using NanoAssistant.Shared.Dtos;
using System.Linq;

namespace NanoAssistant.Core.Grains
{
    public class NanoAssistantGrain : Grain<NanoAssistantState>, INanoAssistantGrain
    {
        private readonly IChatCompletionService _chatCompletionService;
        private readonly Kernel _kernel;
        private readonly PromptExecutionSettings _promptExecutionSettings;

        private readonly HashSet<string> idempotencyKeys = [];

        public NanoAssistantGrain(IChatCompletionService chatCompletionService, Kernel kernel, PromptExecutionSettings promptExecutionSettings)
        {
            _chatCompletionService = chatCompletionService;
            _kernel = kernel;
            _promptExecutionSettings = promptExecutionSettings;
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            State.ChatHistory.AddSystemMessage("You are a personal assistant named Nano assistant.");
            State.ChatHistory.AddSystemMessage("Use dollars when dealing with money.");
            State.ChatHistory.AddSystemMessage("When adding expense or income get the balance.");
            return base.OnActivateAsync(cancellationToken);
        }

        public async Task<ChatDto> AddUserMessage(UserMessageDto userMessage)
        {
            if (idempotencyKeys.Contains(userMessage.IdempotencyKey))
            {
                var lastChat = State.ChatHistory.LastOrDefault();
                
                return new ChatDto
                {
                    Role = lastChat?.Role.ToString() ?? string.Empty,
                    Message = lastChat?.ToString() ?? string.Empty,
                };
            }
            State.ChatHistory.AddUserMessage(userMessage.Message);
            var result = await _chatCompletionService.GetChatMessageContentAsync(State.ChatHistory, _promptExecutionSettings, _kernel);
            State.ChatHistory.Add(result);
            await WriteStateAsync();
            idempotencyKeys.Add(userMessage.IdempotencyKey);
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
                Chats = State.ChatHistory
                .Where(chat => !string.IsNullOrEmpty(chat.ToString()) && (chat.Role == AuthorRole.Assistant || chat.Role == AuthorRole.User))
                .Select(chat => new ChatDto()
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

        public HashSet<string> IdempotencyKeys { get; set; } = [];
    }
}
