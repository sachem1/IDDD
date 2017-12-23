using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Coralcode.Framework.WCF.Json
{
    public class RawBodyWriter : BodyWriter
    {
        readonly byte[] _content;
        public RawBodyWriter(byte[] content)
            : base(true)
        {
            _content = content;
        }

        public RawBodyWriter(object content)
            : base(true)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                {
                    using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
                    {
                        writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                        serializer.Serialize(writer, content);
                        sw.Flush();
                        _content = ms.ToArray();
                    }
                }
            }
        }




        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("Binary");
            if (_content != null)
                writer.WriteBase64(_content, 0, _content.Length);
            writer.WriteEndElement();
        }
    }
}
