using System;
using Newtonsoft.Json;

namespace Slack.Common
{
    public class JavascriptDateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = double.Parse(reader.Value.ToString());
            return JsonToDate(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(DateToJson((DateTime)value));
        }

        public static DateTime JsonToDate(double value)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).Add(TimeSpan.FromSeconds(value)).ToLocalTime();
        }

        public static double DateToJson(DateTime value)
        {
            return value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }
}