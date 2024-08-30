using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoAssistant.Core.Dtos
{
    [GenerateSerializer]
    public class ChatHistoryDto
    {
        [Id(0)]
        public List<ChatDto> Chats { get; set; } = new List<ChatDto>();
    }
}
