using System.Text.Json.Nodes;

namespace NanoAssistant.BlazorWebApp.Client.MessageParsers
{
    public interface IMessageParser
    {
        bool CanParseMessage(JsonNode jsonNode);

        IParsedMessage? ParseMessage(JsonNode jsonNode);
    }
}
