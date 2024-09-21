using NanoAssistant.BlazorWebApp.Client.MessageParsers;

namespace NanoAssistant.BlazorWebApp.Client.Models
{
    public class ChartMessage: IParsedMessage
    {
        public string Type { get; set; } = string.Empty;

        public List<ChartData> Data { get; set; } = new List<ChartData>();

        public string[] Lables { get => Data.Select(c => c.Label).ToArray(); }

        public double[] Values { get => Data.Select(c => (double)Math.Abs(c.Value)).ToArray(); }

    }

    public class ChartData
    {
        public string Label { get; set; } = string.Empty;

        public int Value { get; set; }
    }
}
