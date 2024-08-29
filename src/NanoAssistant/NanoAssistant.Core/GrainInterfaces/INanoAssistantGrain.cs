using NanoAssistant.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoAssistant.Core.GrainInterfaces
{
    public interface INanoAssistantGrain : IGrainWithStringKey
    {
        Task<string> AddUserMessage(UserMessage userMessage);
    }
}
