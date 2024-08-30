using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using NanoAssistant.Core.Dtos;
using NanoAssistant.Core.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> AddUserMessage(UserMessage userMessage)
        {
            State.ChatHistory.AddUserMessage(userMessage.Message);
            var result = await _chatCompletionService.GetChatMessageContentAsync(State.ChatHistory, _promptExecutionSettings, _kernel);
            State.ChatHistory.Add(result);
            return result.ToString();
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
