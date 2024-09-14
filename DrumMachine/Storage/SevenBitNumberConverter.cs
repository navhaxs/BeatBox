using System;
using System.Collections.Generic;
using System.Linq;
using Melanchall.DryWetMidi.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DrumMachine.Storage;

public class SevenBitNumberConverter : JsonConverter
{
    private readonly Type[] _types;

    public SevenBitNumberConverter(params Type[] types)
    {
        _types = types;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JToken t = JToken.FromObject(value);

        // if (t.Type != JTokenType.Object)
        // {
            t.WriteTo(writer);
        // }
        // else
        // {
        //     JObject o = (JObject)t;
        //     if (value is SevenBitNumber)
        //     {
        //         writer.WriteValue(((Color)xmlColor).ToString());
        //     }
            // IList<string> propertyNames = o.Properties().Select(p => p.Name).ToList();
            //
            // o.AddFirst(new JProperty("Keys", new JArray(propertyNames)));
            //
            // o.WriteTo(writer);
        // }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Integer)
        {
            long value = reader.Value != null ? (long)reader.Value : 0;
            return new SevenBitNumber((byte)value);
        }
        
        throw new JsonSerializationException($"Unexpected token {reader.TokenType} when reading SevenBitNumber.");
    }

    public override bool CanConvert(Type objectType)
    {
        return _types.Any(t => t == objectType);
    }
}