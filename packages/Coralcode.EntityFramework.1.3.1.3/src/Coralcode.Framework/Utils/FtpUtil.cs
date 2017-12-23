using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Log;

namespace Coralcode.Framework.Utils
{
    public class FtpUtil
    {
        private string Server;
        private string RootPath;
        private string User;
        private string Password;
        private string FtpUrl;
        private string ReadUrl;

        public static readonly FtpUtil Default = new FtpUtil();

        public static FtpUtil CreatFtpUtil(string server, string rootPath, string user, string password, string readUrl = "")
        {
            var ftpUtil = new FtpUtil();
            ftpUtil.Server =server.Replace("ftp://","");
            ftpUtil.RootPath = rootPath;
            ftpUtil.ReadUrl = string.IsNullOrEmpty(readUrl) ? string.Format("http://{0}/{1}", ftpUtil.Server, ftpUtil.RootPath) : readUrl;
            ftpUtil.User = user;
            ftpUtil.Password = password;
            ftpUtil.FtpUrl = string.IsNullOrEmpty(ftpUtil.RootPath) ? string.Format("ftp://{0}", ftpUtil.Server) : string.Format("ftp://{0}/{1}", ftpUtil.Server, ftpUtil.RootPath);
            return ftpUtil;
        }

        private FtpUtil()
        {
            ReadUrl = AppConfig.DllConfigs.Current["Ftp"]["ReadServer"];
            Server = AppConfig.DllConfigs.Current["Ftp"]["Server"];
            RootPath = AppConfig.DllConfigs.Current["Ftp"]["RootPath"];
            User = AppConfig.DllConfigs.Current["Ftp"]["User"];
            Password = AppConfig.DllConfigs.Current["Ftp"]["Password"];
            FtpUrl = string.Format("ftp://{0}/{1}", Server, RootPath);
        }



        public string GetFtpUrl()
        {
            return FtpUrl; ;
        }
        public string GetReadUrl()
        {
            return ReadUrl;
        }

        public string ChangeFtpUrlToReadUrl(string url)
        {
            return url.Replace(FtpUrl, ReadUrl);
        }

        #region 文件相关操作
        /// <summary>
        /// 获取当前目录下文件列表(仅文件)
        /// </summary>
        /// <returns></returns>
        public List<string> GetFileList(string path)
        {
            var drectory = GetFilesDetailList(path);
            if (drectory == null || drectory.Count == 0)
                return new List<string>();
            var files = new List<string>();
            foreach (string str in drectory)
            {
                var parts = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                //解析出来至少有四部分,data item  size  filename这样的格式
                if (parts.Length < 4)
                    continue;

                //判断第三部分如果是dir则为目录直接忽略
                if (parts[2].Trim() == "<DIR>")
                    continue;
                var temp = str;
                parts.Take(3).ForEach(item =>
                {
                    temp = temp.Replace(item, "").Trim();
                });
                files.Add(temp);
            }
            return files;
        }

        public void CreateDirectoryAndUploadFile(Stream stream, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            

            var dir = FtpPathUtil.GetDirectory(filePath);
            if (!IsDirectoryExist(dir))
                CreateDirectory(dir);
            var request = GetRequest(filePath, WebRequestMethods.Ftp.UploadFile, true, stream.Length);
            var strm = request.GetRequestStream();
            const int buffLength = 2048;
            var buff = new byte[buffLength];
            stream.Seek(0, SeekOrigin.Begin);
            var contentLen = stream.Read(buff, 0, buffLength);
            while (contentLen != 0)
            {
                strm.Write(buff, 0, contentLen);
                contentLen = stream.Read(buff, 0, buffLength);
            }
            strm.Close();


        }

        public void CreateDirectoryAndUploadFile(byte[] bytes, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
          

            var dir = FtpPathUtil.GetDirectory(filePath);
            if (!IsDirectoryExist(dir))
                CreateDirectory(dir);
            var request = GetRequest(filePath, WebRequestMethods.Ftp.UploadFile, true, bytes.LongLength);
            var strm = request.GetRequestStream();

            const int buffLength = 2048;


            var chunkCount = bytes.LongLength / buffLength;
            var chunkLastNumber = bytes.LongLength % buffLength;

            for (int i = 0; i < chunkCount; i++)
            {
                int startIndex = i * buffLength;
                strm.Write(bytes, startIndex, buffLength);
            }
            if (chunkLastNumber > 0)
            {
                strm.Write(bytes, Convert.ToInt32(chunkCount * buffLength), Convert.ToInt32(chunkLastNumber));
            }
            strm.Close();


        }

        public void FileUpload(Stream stream, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
         

            var reqFtp = GetRequest(filePath, WebRequestMethods.Ftp.UploadFile, true, stream.Length);
            Stream strm = reqFtp.GetRequestStream();
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            stream.Seek(0, SeekOrigin.Begin);
            var contentLen = stream.Read(buff, 0, buffLength);
            while (contentLen != 0)
            {
                strm.Write(buff, 0, contentLen);
                contentLen = stream.Read(buff, 0, buffLength);
            }
            strm.Close();

        }
        public Stream DownloadFile(string filePath)
        {

            var response = SendRequest(filePath, WebRequestMethods.Ftp.DownloadFile);
            var ftpStream = response.GetResponseStream();
            var memeryStream = new MemoryStream();
            var bufferSize = 2048;
            byte[] buffer = new byte[bufferSize];
            if (ftpStream != null)
            {
                var readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    memeryStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
            }
            response.Close();
            return memeryStream;

        }
        public void RemoveFile(string filePath)
        {
            var response = SendRequest(filePath, WebRequestMethods.Ftp.DeleteFile);
            //long size = response.ContentLength;
            Stream datastream = response.GetResponseStream();
            if (datastream != null)
            {
                StreamReader sr = new StreamReader(datastream);
                sr.ReadToEnd();
                sr.Close();
                datastream.Close();
            }
            response.Close();

        }
        #endregion

        #region 目录相关操作
        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsDirectoryExist(string path)
        {

            if (string.IsNullOrEmpty(path))
                return false;

            var folders = path.Split('/');
            

            var requestPath = string.Empty;

            for (var index = 0; index < folders.Length ; index++)
            {
                var folder = folders[index];

                var directories = GetDirectoryList(requestPath);

                if (!directories.Contains(folder))
                    return false;
                requestPath += "/" + folder;
            }
            return true;
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path"></param>
        public void CreateDirectory(string path)
        {
            var parts = FtpPathUtil.SplitPaths(path);
            string url = string.Empty;
            parts.ForEach(part =>
            {
                url = FtpPathUtil.Combine(url, part);
                if (!IsDirectoryExist(url))
                {
                    CreateChildDirectory(url);
                }
            });
        }

        /// <summary>
        /// 创建文件夹,只能创建一级
        /// 如果要创建 path1/path2/path3
        /// 则path1/path2  比如存在
        /// </summary>
        /// <param name="path"></param>
        private void CreateChildDirectory(string path)
        {

            var response = SendRequest(path, WebRequestMethods.Ftp.MakeDirectory, true);
            var ftpStream = response.GetResponseStream();
            if (ftpStream != null) ftpStream.Close();
            response.Close();

        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="path"></param>
        public void RemoveDirectory(string path)
        {
            if (!IsDirectoryExist(path))
                return;

            var response = SendRequest(path, WebRequestMethods.Ftp.RemoveDirectory);
            Stream datastream = response.GetResponseStream();
            if (datastream != null)
            {
                StreamReader sr = new StreamReader(datastream);
                sr.ReadToEnd();
                sr.Close();
                datastream.Close();
            }
            response.Close();

        }

        /// <summary>
        /// 获取当前目录下所有的文件夹列表(仅文件夹)
        /// </summary>
        /// <returns></returns>
        public List<string> GetDirectoryList(string path)
        {
            var drectory = GetFilesDetailList(path);
            if (drectory == null || drectory.Count == 0)
                return new List<string>();
            var result = new List<string>();
            foreach (string str in drectory)
            {
                int dirPos = str.IndexOf("<DIR>", StringComparison.Ordinal);
                if (dirPos > 0)
                {
                    /*判断 Windows 风格*/

                    result.Add(str.Substring(dirPos + 5).Trim());
                }
                else if (str.Trim().Substring(0, 1).ToUpper() == "D")
                {
                    /*判断 Unix 风格*/
                    string dir = str.Substring(54).Trim();
                    if (dir != "." && dir != "..")
                    {
                        result.Add(dir);
                    }
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取当前目录下明细(包含文件和文件夹)
        /// </summary>
        /// <returns></returns>
        private List<string> GetFilesDetailList(string path)
        {

            var result = new List<string>();
            var response = SendRequest(path, WebRequestMethods.Ftp.ListDirectoryDetails);
            var ftpStream = response.GetResponseStream();
            if (ftpStream == Stream.Null)
                return new List<string>();

            if (ftpStream != null)
            {
                var reader = new StreamReader(ftpStream, Encoding.UTF8);
                var line = reader.ReadLine();
                while (line != null)
                {
                    result.Add(line);
                    line = reader.ReadLine();
                }
                if (string.IsNullOrEmpty(result.ToString()))
                    return new List<string>();
                reader.Close();
                ftpStream.Close();
            }
            response.Close();
            return result;

        }

        private WebResponse SendRequest(string path, string method, bool useBinary = false, long contentLength = 0)
        {
            LoggerFactory.Instance.Info(path);
            string ftpUri;
            if (string.IsNullOrEmpty(path))
                ftpUri = FtpUrl;
            else if (path.StartsWith(ReadUrl))
            {
                ftpUri = string.Format("{0}{1}", FtpUrl, path.Replace(ReadUrl, ""));
            }
            else if (path.StartsWith(RootPath)&&!string.IsNullOrEmpty(RootPath))
                ftpUri = FtpPathUtil.Combine(FtpUrl.Replace("/" + RootPath, ""), path);
            else if (path.StartsWith(string.Format("ftp://{0}", Server)))
                ftpUri = path;
            else if (path.StartsWith(FtpUrl))
                ftpUri = path;
            else if (path.StartsWith(string.Format("/{0}", RootPath)))
                ftpUri = FtpPathUtil.Combine(FtpUrl.Replace("/" + RootPath, ""), path);
            else
                ftpUri = FtpPathUtil.Combine(FtpUrl, path);

            LoggerFactory.Instance.Info(ftpUri);
            var ftp = (FtpWebRequest)WebRequest.Create(new Uri(ftpUri));
            ftp.Credentials = new NetworkCredential(User, Password);
            ftp.Method = method;
            ftp.UseBinary = useBinary;
            ftp.ContentLength = contentLength;
            return ftp.GetResponse();
        }

        private WebRequest GetRequest(string path, string method, bool useBinary = false, long contentLength = 0)
        {
            string ftpUri;
            if (!string.IsNullOrEmpty(path) && path.StartsWith("/"))
                path = path.Substring(1);
            if (string.IsNullOrEmpty(path))
                ftpUri = FtpUrl;
            else if (path.StartsWith(RootPath))
                ftpUri = FtpPathUtil.Combine(FtpUrl.Replace("/" + RootPath, ""), path);
            else if (path.StartsWith(string.Format("ftp://{0}", Server)))
                ftpUri = path;
            else
                ftpUri = FtpPathUtil.Combine(FtpUrl, path);
            var ftp = (FtpWebRequest)WebRequest.Create(new Uri(ftpUri));
            ftp.Credentials = new NetworkCredential(User, Password);
            ftp.Method = method;
            ftp.UseBinary = useBinary;
            ftp.ContentLength = contentLength;
            return ftp;
        }

        /// <summary>
        /// 将http转成ftp
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <returns></returns>
        public string ReplaceProtocolHttp2Ftp(string httpUrl)
        {
            string ftpUri;
            if (string.IsNullOrEmpty(httpUrl))
                ftpUri = FtpUrl;
            else if (httpUrl.StartsWith(ReadUrl))
            {
                ftpUri = string.Format("{0}{1}", FtpUrl, httpUrl.Replace(ReadUrl, ""));
            }
            else if (httpUrl.StartsWith(RootPath))
                ftpUri = FtpPathUtil.Combine(FtpUrl.Replace("/" + RootPath, ""), httpUrl);
            else if (httpUrl.StartsWith(string.Format("ftp://{0}", Server)))
                ftpUri = httpUrl;
            else if (httpUrl.StartsWith(FtpUrl))
                ftpUri = httpUrl;
            else if (httpUrl.StartsWith(string.Format("/{0}", RootPath)))
                ftpUri = FtpPathUtil.Combine(FtpUrl.Replace("/" + RootPath, ""), httpUrl);
            else
                ftpUri = FtpPathUtil.Combine(FtpUrl, httpUrl);

            return ftpUri;
        }

    }
}
