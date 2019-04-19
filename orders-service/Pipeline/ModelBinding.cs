using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Serverless
{
    public static class ModelBinding
    {
        public static T Deserialize<T>(this HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body))
            using (var textReader = new JsonTextReader(reader))
            {
                request.Body.Seek(0, SeekOrigin.Begin);

                var serializer = JsonSerializer.Create(new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                return serializer.Deserialize<T>(textReader);
            }
        }
    }
}