namespace NanoAssistant.Shared.Dtos
{
    public class UserMessageDto
    {
        public string IdempotencyKey { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
