using System.ServiceModel.Channels;

namespace Coralcode.Framework.WCF.Json
{
    public class JsonRawMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            return WebContentFormat.Raw;
        }
    }
}
