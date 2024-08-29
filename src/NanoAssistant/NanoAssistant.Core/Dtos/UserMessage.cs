using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoAssistant.Core.Dtos
{
    [GenerateSerializer]
    public class UserMessage
    {
        [Id(0)]
        public string Message { get; set; } = string.Empty;
    }
}
