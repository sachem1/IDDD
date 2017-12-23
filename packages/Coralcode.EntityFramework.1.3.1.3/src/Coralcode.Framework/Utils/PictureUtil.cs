using System;
using System.Drawing;
using System.IO;

namespace Coralcode.Framework.Utils
{
    public class PictureUtil
    {

        #region 私有方法
        /// <summary>
        ///将图片编码为64位编码
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }

        /// <summary>
        /// 生成缩略图的路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public  static string GeneralThumbnailFileName(string fileName)
        {
            return string.Format("{0}_thumbnail{1}", Path.GetFileNameWithoutExtension(fileName),
               Path.GetExtension(fileName));
        }
        /// <summary>
        /// 生成缩略图的路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GeneralEnlargedFileName(string fileName)
        {
            return string.Format("{0}_enlarged{1}", Path.GetFileNameWithoutExtension(fileName),
               Path.GetExtension(fileName));
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalStream"></param>
        /// <param name="thumbnailStream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        public static void MakeThumbnail(Stream originalStream, int width, int height, string mode,out Stream thumbnailStream)
        {
            thumbnailStream = new MemoryStream();
            originalStream.Seek(0, SeekOrigin.Begin);
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(originalStream);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                 
                    break;
                case "W"://指定宽，高按比例                     
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例 
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                 
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片 
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板 
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图 
                bitmap.Save(thumbnailStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
        #endregion

    }
}
