using System;
using System.Linq;
using System.Text;

namespace Coralcode.Framework.Utils
{
    public class FtpPathUtil
    {
        private const string SplitString = "/";

        public static string Combine(params string[] paths)
        {
            var builder = new StringBuilder();
            foreach (var path in paths.Where(item=>!string.IsNullOrEmpty(item)))
            {
                builder.AppendFormat("{0}/", path);
            }
            var result = builder.ToString();
            if (result.EndsWith(SplitString))
               result= result.Remove(result.LastIndexOf(SplitString, StringComparison.Ordinal), 1);
            return result;
        }

        public static bool IsFtpAddress(string path)
        {
            return path.StartsWith("ftp://");
        }

        public static string[] SplitPaths(string path)
        {
            return path.Split(new[] { SplitString },StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetDirectory(string filePath)
        {
            Uri uri;
            string path = Uri.TryCreate(filePath, UriKind.Absolute, out uri) ? uri.AbsolutePath : filePath;
            if (!path.Contains("/") || path.EndsWith("/"))
            {
                return path;
            }
            return path.Substring(0, path.LastIndexOf(SplitString, StringComparison.Ordinal));
        }

        public static string GetPathWithOutHost(string filePath)
        {
            Uri uri;
            string path = Uri.TryCreate(filePath, UriKind.Absolute, out uri) ? uri.AbsolutePath : filePath;
            if (!path.Contains("/") || path.EndsWith("/"))
            {
                return path;
            }
            return path;
        }
    }
}
