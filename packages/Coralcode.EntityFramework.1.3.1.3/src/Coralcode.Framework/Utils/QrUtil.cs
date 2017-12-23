using System.Drawing.Imaging;
using System.IO;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace Coralcode.Framework.Utils
{
    public class QrUtil
    {

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="ms">写入的文件流</param>
        /// <returns></returns>
        public static bool General(string content, MemoryStream ms)
        {
            QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.M);
            QuietZoneModules QuietZones = QuietZoneModules.Two;  //空白区域   
            int ModuleSize = 12;//大小  
            QrCode code;
            if (encoder.TryEncode(content, out code))
            {
                var render = new GraphicsRenderer(new FixedModuleSize(ModuleSize, QuietZones));
                render.WriteToStream(code.Matrix, ImageFormat.Jpeg, ms);
                return true;
            }
            return false;
        }
    }
}
