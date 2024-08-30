using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoAssistant.Core.Dtos
{
    [GenerateSerializer]
    public class ChatDto
    {
        [Id(0)]
        public string Role { get; set; } = string.Empty;

        [Id(1)]
        public string Message { get; set; } = string.Empty;
    }
}
