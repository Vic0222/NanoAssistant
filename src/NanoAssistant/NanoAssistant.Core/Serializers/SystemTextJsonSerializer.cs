using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NanoAssistant.Core.Serializers
{
    public class SystemTextJsonSerializer : IGrainStorageSerializer
    {
        public T Deserialize<T>(BinaryData input)
        {
            return JsonSerializer.Deserialize<T>(input.ToStream());
        }

        public BinaryData Serialize<T>(T input)
        {
            return new BinaryData(JsonSerializer.Serialize<T>(input));
        }
    }
}
