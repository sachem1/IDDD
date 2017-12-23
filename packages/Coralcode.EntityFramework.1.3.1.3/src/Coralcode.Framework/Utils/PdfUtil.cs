using System;
using System.IO;
using System.Text;
using Coralcode.Framework.ConfigManager;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;

namespace Coralcode.Framework.Utils
{
    public class PdfUtil
    {
        /// <summary>  
        /// 将Html文字 输出到PDF档里  
        /// </summary>  
        /// <param name="htmlText">html文本</param>
        /// <param name="param">pdf大小</param>
        /// <param name="waterMark">水印</param>
        /// <param name="stampPoint">模拟盖章</param>
        /// <returns></returns>  
        public static byte[] ConvertHtmlTextToPdf(string htmlText, DocumentParam param, WatermarkSetting waterMark =null, StampPoint stampPoint=null)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return null;
            }
            //避免当htmlText无任何html tag标签的纯文字时，转PDF时会挂掉，所以一律加上<p>标签  
            htmlText = "<p>" + htmlText + "</p>";

            MemoryStream outputStream = new MemoryStream();//要把PDF写到哪个串流  
            byte[] data = Encoding.UTF8.GetBytes(htmlText.ToCharArray());//字串转成byte[]  
            MemoryStream msInput = new MemoryStream(data);
            Document doc = new Document(param.PageSize, param.MarginLeft, param.MarginRight, param.MarginTop, param.MarginBottom); ;//要写PDF的文件，建构子没填的话预设直式A4 
            PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
            //writer.SetViewerPreferences(PdfWriter.HideMenubar | PdfWriter.HideToolbar); 
            //指定文件预设开档时的缩放为100%  

            PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            //开启Document文件   
            doc.Open();
            if (stampPoint!=null&&!string.IsNullOrEmpty(stampPoint.Path))
            {
                var img = Image.GetInstance(stampPoint.Path);
                img.SetAbsolutePosition(doc.PageSize.Width - img.Width - stampPoint.X, img.Height + stampPoint.Y);
                doc.Add(img);
            }

            //使用XMLWorkerHelper把Html parse到PDF档里  
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new UnicodeFontFactory());
            //XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8);

            //将pdfDest设定的资料写到PDF档  
            var action = PdfAction.GotoLocalPage(1, pdfDest, writer);
            writer.SetOpenAction(action);
            doc.Close();
            msInput.Close();
            outputStream.Close();

            return SetWatermark(waterMark, outputStream.ToArray());
        }


        private static byte[] SetWatermark(WatermarkSetting waterMark, byte[] bytes)
        {
            if (waterMark==null||string.IsNullOrEmpty(waterMark.Text)) return bytes;
            var gs = new PdfGState();
            var pdfReder = new PdfReader(bytes);
            var fileName=DateTime.Now.ToFileTime()+Guid.NewGuid().ToString()+".pdf";
            var outPutPdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", fileName);
            var pdfStamper = new PdfStamper(pdfReder, new FileStream(outPutPdfPath,FileMode.Create));
            try
            {
                var fontPath = AppConfig.GetFileByAbsolutePath("STCAIYUN.TTF");
                var font = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                for (var i = 1; i <= pdfReder.NumberOfPages; i++)
                {
                    var width = pdfReder.GetPageSize(i).Width;
                    var height = pdfReder.GetPageSize(i).Height;
                    var content = pdfStamper.GetOverContent(i);
                    //透明度
                    gs.FillOpacity = 0.4f;
                    content.SetGState(gs);
                    //开始写入文本
                    content.BeginText();
                    content.SetColorFill(BaseColor.BLACK);
                    content.SetFontAndSize(font, waterMark.Size);
                    content.SetTextMatrix(5, 10);
                    content.ShowTextAligned(Element.ALIGN_CENTER, waterMark.Text, width / 2, height / 2 - 50, 35);
                    content.SetColorFill(BaseColor.BLACK);
                    content.EndText();
                }
            }
            catch (Exception)
            {
                return bytes;
            }
            finally
            {
                pdfStamper.Close();
                pdfReder.Close();

            }

            return GetFileBytes(outPutPdfPath);
        }



        private static byte[] GetFileBytes(string outPutPdfPath)
        {
            var file = new FileInfo(outPutPdfPath);
            if (!file.Exists) return null;
            var stream = file.OpenRead();
            var result = new byte[stream.Length];
            stream.Read(result, 0, (int)stream.Length);
            stream.Close();
            file.Delete();

            return result;
        }
    }



    //设置字体类  
    public class UnicodeFontFactory : FontFactoryImp
    {
        public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color, bool cached)
        {
            var fontPath= AppConfig.GetFileByAbsolutePath("simsun.ttc")+",1";
            BaseFont bfChiness = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return new Font(bfChiness, size, style, color);
        }
    }

    public class WatermarkSetting
    {
        public WatermarkSetting()
        {
            Size = 35;
        }

        /// <summary>
        /// 水印内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public float Size { get; set; }
    }

    public class StampPoint
    {
        public StampPoint ()
        {
            X = 40;
            Y = -100;
        }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// X轴位置
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y轴位置
        /// </summary>
        public float Y { get; set; }
    }

    /// <summary>
    /// 构造文档需要的参数
    /// </summary>
    public class DocumentParam
    {
        public Rectangle PageSize { get; set; }
        public float MarginLeft { get; set; }
        public float MarginRight { get; set; }
        public float MarginTop { get; set; }
        public float MarginBottom { get; set; }
    }
}
