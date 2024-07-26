using Fetcharr.Models.Configuration;
using Fetcharr.Models.Configuration.Radarr;
using Fetcharr.Models.Configuration.Sonarr;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Fetcharr.API.Configuration
{
    public class ServiceInstanceNodeDeserializer : INodeDeserializer
    {
        public bool Deserialize(
            IParser reader,
            Type expectedType,
            Func<IParser, Type, object?> nestedObjectDeserializer,
            out object? value,
            ObjectDeserializer objectDeserializer)
        {
            value = null;

            return expectedType switch
            {
                Type t when t == typeof(FetcharrRadarrConfiguration[])
                    => this.Deserialize<FetcharrRadarrConfiguration>(reader, expectedType, nestedObjectDeserializer, out value, objectDeserializer),

                Type t when t == typeof(FetcharrSonarrConfiguration[])
                    => this.Deserialize<FetcharrSonarrConfiguration>(reader, expectedType, nestedObjectDeserializer, out value, objectDeserializer),

                _ => false
            };
        }

        public bool Deserialize<T>(
            IParser reader,
            Type expectedType,
            Func<IParser, Type, object?> nestedObjectDeserializer,
            out object? value,
            ObjectDeserializer objectDeserializer)
            where T : FetcharrServiceConfiguration
        {
            if(expectedType != typeof(T[]))
            {
                value = null;
                return false;
            }

            if(!reader.TryConsume<MappingStart>(out _))
            {
                value = null;
                return false;
            }

            List<T> result = [];
            while (!reader.TryConsume<MappingEnd>(out _))
            {
                Scalar keyScalar = reader.Consume<Scalar>();
                T? input = nestedObjectDeserializer(reader, typeof(T)) as T;

                if(input is not null)
                {
                    input.Name = keyScalar.Value;
                    result.Add(input);
                }
            }

            value = result.ToArray();
            return true;
        }
    }
}