using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Utils
{
    public static class StreamUtil
    {
        public static byte[] ToArry(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            var memeryStream = new MemoryStream();
            var bufferSize = 2048;
            byte[] buffer = new byte[bufferSize];
            stream.Seek(0, SeekOrigin.Begin);
            var readCount = stream.Read(buffer, 0, bufferSize);
            while (readCount > 0)
            {
                memeryStream.Write(buffer, 0, readCount);
                readCount = stream.Read(buffer, 0, bufferSize);
            }
            stream.Close();
            return memeryStream.ToArray();
        }
    }
}
