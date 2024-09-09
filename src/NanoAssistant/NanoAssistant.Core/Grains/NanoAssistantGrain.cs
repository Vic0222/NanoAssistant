using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using NanoAssistant.Core.GrainInterfaces;
using NanoAssistant.Core.SemanticPlugins;
using NanoAssistant.Core.Services;
using NanoAssistant.Shared.Dtos;
using System.Linq;

namespace NanoAssistant.Core.Grains
{
    public class NanoAssistantGrain : Grain<NanoAssistantState>, INanoAssistantGrain
    {
        private readonly FinanceTrackerPlugin _financeTrackerPlugin;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly Kernel _kernel;
        private readonly PromptExecutionSettings _promptExecutionSettings;
        private readonly HashSet<string> idempotencyKeys = [];

        public NanoAssistantGrain(FinanceTrackerPlugin financeTrackerPlugin, IChatCompletionService chatCompletionService, Kernel kernel, PromptExecutionSettings promptExecutionSettings)
        {
            _financeTrackerPlugin = financeTrackerPlugin;
            _chatCompletionService = chatCompletionService;
            _kernel = kernel;
            _promptExecutionSettings = promptExecutionSettings;
        }

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            AddSystemMessages();

            _kernel.Plugins.AddFromObject(_financeTrackerPlugin);

            return base.OnActivateAsync(cancellationToken);
        }

        private void AddSystemMessages()
        {
            State.ChatHistory.AddSystemMessage("You are a personal assistant named Nano assistant.");
            State.ChatHistory.AddSystemMessage($"Today is {DateTimeOffset.Now.ToString("d")}.");
            State.ChatHistory.AddSystemMessage("Use dollars when dealing with money.");
            State.ChatHistory.AddSystemMessage("When asking for balance return the financial summary in markdown.");
        }

        public async Task<ChatDto> AddUserMessage(UserMessageDto userMessage, string accessToken)
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

            _financeTrackerPlugin.SetAccessToken(accessToken);

            //change chat history to last 20.
            var newChatHistory = State.ChatHistory.Skip(Math.Max(0, State.ChatHistory.Count - 100)).SkipWhile(chat => chat.Role == AuthorRole.Tool);
            State.ChatHistory = new ChatHistory(newChatHistory);
            AddSystemMessages();
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
