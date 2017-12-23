using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coralcode.Framework.Serializers.JsonConverter
{
    public class StreamToByteConverter : Newtonsoft.Json.JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jt = JValue.ReadFrom(reader);
            var base64String = jt.Value<string>();
            if (string.IsNullOrEmpty(base64String))
                return null;
            return new MemoryStream(Convert.FromBase64String(base64String)); 
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Stream) == objectType;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var datas = ((Stream) value).ToArry();
            serializer.Serialize(writer, datas);
        }
    }
}
