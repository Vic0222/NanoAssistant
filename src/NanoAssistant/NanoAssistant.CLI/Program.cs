// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using NanoAssistant.Core.SemanticPlugins;

Console.WriteLine("Hello, World!");

var configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddUserSecrets<Program>();

var configuration =configurationBuilder.Build();

var builder = Kernel.CreateBuilder();

// Add builder configuration and services
builder.AddOpenAIChatCompletion("gpt-4o-mini", configuration["OpenAPIKey"]);
builder.Plugins.AddFromType<FinanceTrackerPlugin>("FinanceTracker");

var kernel = builder.Build();
var chat = kernel.GetRequiredService<IChatCompletionService>();
ChatHistory chatHistory = [];
chatHistory.AddSystemMessage("You are a friendly personal assitant named Nano assistant.");


OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

while (true)
{
    string input = Console.ReadLine();
    chatHistory.AddUserMessage(input);
    var result = await chat.GetChatMessageContentAsync(chatHistory, executionSettings: openAIPromptExecutionSettings, kernel: kernel);
    chatHistory.Add(result);
    Console.WriteLine($"assistant: {result}");
}
