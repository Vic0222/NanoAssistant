using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoAssistant.Core.Services
{
    public interface ITokenStorageService
    {
        string AccessToken { get; set; }
    }

    public class TokenStorageService : ITokenStorageService
    {
        public string AccessToken { get; set; } = string.Empty;
    }
}
