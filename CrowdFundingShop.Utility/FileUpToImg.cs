using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CrowdFundingShop.Utility
{
    public class FileUpToImg
    {
        private String _imageServerPath = Config.ImagePath;
        private String _ftpUser = Config.FtpUser;
        private String _ftpPassword = Config.FtpPassword;
        protected String pathName = String.Empty;//上传到的服务器路径

        private String _ftpuri = String.Empty;
        private String _uploadurl = String.Empty;

        /// <summary>
        /// 上传文件的访问地址
        /// </summary>
        public String uploadUrl { get { return _uploadurl; } }
        /// <summary>
        /// 上传文件的ftp访问地址
        /// </summary>
        public String ftpUri { get { return _ftpuri; } }

        /// <summary>
        /// 将图片通过FTP上传到服务器
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="extendname">图片后缀名</param>
        /// <param name="pathName">图片保存的文件夹名</param>
        public FileUpToImg(Stream stream, String extendname, String pathName)
        {
            this.pathName = pathName;
            string month = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString();
            this.pathName = String.IsNullOrEmpty(this.pathName) ? "Aothers" : this.pathName;
            this._ftpuri = String.Format("ftp://{0}/{1}/{2}/{3}.{4}", this._imageServerPath, this.pathName, month, DateTime.Now.ToString("yyyyMMddhhmmssmmm"), extendname);
            String uripath = String.Format("ftp://{0}/{1}/", this._imageServerPath, this.pathName);
            if (!IsDir(uripath)) CreateDir(uripath);
            if (!IsDir(this._ftpuri)) CreateDir(this._ftpuri);

            // 根据uri创建FtpWebRequest对象 
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(this._ftpuri));

            // ftp用户名和密码 
            reqFTP.Credentials = new NetworkCredential(this._ftpUser, this._ftpPassword);

            // 默认为true，连接不会被关闭 
            // 在一个命令之后被执行 
            reqFTP.KeepAlive = false;

            // 指定执行什么命令 
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // 指定数据传输类型 
            reqFTP.UseBinary = true;

            // 上传文件时通知服务器文件的大小 
            reqFTP.ContentLength = stream.Length;

            // 缓冲大小设置为2kb 
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            try
            {
                // 把上传的文件写入流 
                Stream strm = reqFTP.GetRequestStream();

                // 每次读文件流的2kb                 
                contentLen = stream.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入 upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = stream.Read(buff, 0, buffLength);
                }
                stream.Close();
                strm.Close();
                this._uploadurl = this._ftpuri.Replace(String.Format("ftp://{0}", this._imageServerPath), String.Format("http://{0}", this._imageServerPath));
            }
            catch
            {
                this._uploadurl = String.Empty;
            }


        }

        /// <summary>
        /// 创建文件或目录
        /// </summary>
        /// <param name="fpath"></param>
        /// <returns></returns>
        public bool CreateDir(string fpath)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(fpath.Remove(fpath.LastIndexOf("/"))));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(this._ftpUser, this._ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                WebResponse response = reqFTP.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 目录是否存在
        /// </summary>
        /// <param name="fpath"></param>
        /// <returns></returns>
        public bool IsDir(string fpath)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(fpath.Remove(fpath.LastIndexOf("/"))));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(this._ftpUser, this._ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = reqFTP.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(responseStream, System.Text.Encoding.Default);
                string aa = readStream.ReadToEnd();
                if (aa == "") return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
