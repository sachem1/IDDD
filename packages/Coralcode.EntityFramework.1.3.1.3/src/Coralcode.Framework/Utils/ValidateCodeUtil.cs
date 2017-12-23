using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Coralcode.Framework.Utils
{
    public class ValidateCodeHelper
    {
        /// <summary>
        /// 产生验证码
        /// </summary>
        /// <param name="codeLength"></param>
        /// <returns></returns>
        public static string CreateCode(int codeLength) {
            var codes = new[]{'1','2','3','4','5','6','7','8','9','0',
                              'a','b','c','d','e','f','g','h','i','j',
                              'k','l','m','n','o','p','q','r','s','t',
                              'u','v','w','x','y','z','A','B','C','D',
                              'E','F','G','H','I','J','K','L','M','N',
                              'O','P','Q','R','S','T','U','V','W','X',
                              'Y','Z'};

            var validateCode = string.Empty;
            var rand = new Random();
            for (int i = 0; i < codeLength; i++)
            {
                validateCode += codes[rand.Next(0, codes.Length)];
            }
            return validateCode;
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        public static byte[] CreateValidateGraphic(string validateCode)
        {
            var image = new Bitmap((int)Math.Ceiling(validateCode.Length * 12.0), 22);
            var g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                var random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                var font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                var brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                 Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);
                //画图片的前景干扰点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                var stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}