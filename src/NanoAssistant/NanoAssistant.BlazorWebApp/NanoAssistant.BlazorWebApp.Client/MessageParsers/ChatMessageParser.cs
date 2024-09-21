using NanoAssistant.BlazorWebApp.Client.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NanoAssistant.BlazorWebApp.Client.MessageParsers
{
    public class ChartMessageParser : IMessageParser
    {
        private readonly HashSet<string> _chartTypes;

        public ChartMessageParser()
        {
            _chartTypes = new HashSet<string>() {
                "pie"
            };
        }

        public bool CanParseMessage(JsonNode jsonNode)
        {

            return _chartTypes.Contains(jsonNode["type"]?.GetValue<string>() ?? "");
        }

        public IParsedMessage? ParseMessage(JsonNode jsonNode)
        {
            return jsonNode.Deserialize<ChartMessage>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }
    }

}
