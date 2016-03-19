using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;

namespace CrowdFundingShop.Utility
{
    public class FileUpToImg
    {
        private String _imageServerPath = Config.ImagePath;
        private String _ftpUser = Config.FtpUser;
        private String _ftpPassword = Config.FtpPassword;
        private String pathName = String.Empty;//上传到的服务器路径

        private String _ftpurl = String.Empty;
        private String _uploadurl = String.Empty;
        private String _thumbImageUrl = String.Empty;

        /// <summary>
        /// 上传文件的访问地址
        /// </summary>
        public String uploadUrl { get { return _uploadurl; } }
        /// <summary>
        /// 上传文件的ftp访问地址
        /// </summary>
        public String ftpUrl { get { return _ftpurl; } }

        /// <summary>
        /// 缩略图地址
        /// </summary>
        public String thumbImageUrl { get { return _thumbImageUrl; } }

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
            this._ftpurl = String.Format("ftp://{0}/{1}/{2}/{3}.{4}", this._imageServerPath, this.pathName, month, DateTime.Now.ToString("yyyyMMddhhmmssmmm"), extendname);
            String uripath = String.Format("ftp://{0}/{1}/", this._imageServerPath, this.pathName);
            if (!IsDir(uripath)) CreateDir(uripath);
            if (!IsDir(this._ftpurl)) CreateDir(this._ftpurl);

            // 根据uri创建FtpWebRequest对象 
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(this._ftpurl));

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
                this._uploadurl = this._ftpurl.Replace(String.Format("ftp://{0}", this._imageServerPath), String.Format("http://{0}", this._imageServerPath));

                string thumbImagePath = System.Web.HttpContext.Current.Server.MapPath("imges");
            }
            catch
            {
                this._uploadurl = String.Empty;
            }


        }

        public FileUpToImg()
        {
            // TODO: Complete member initialization
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

        /// <summary>
        /// asp.net上传图片并生成缩略图
        /// </summary>
        /// <param name="upImage">HtmlInputFile控件</param>
        /// <param name="sSavePath">保存的路径,些为相对服务器路径的下的文件夹</param>
        /// <param name="sThumbExtension">缩略图的thumb</param>
        /// <param name="intThumbWidth">生成缩略图的宽度</param>
        /// <param name="intThumbHeight">生成缩略图的高度</param>
        /// <returns>缩略图名称</returns>
        public string UpLoadImage(HttpPostedFileBase upImage, string sSavePath, string sThumbExtension, int intThumbWidth, int intThumbHeight)
        {
            string sThumbFile = "";
            string sFilename = "";
            if (upImage!= null)
            {
                int nFileLen = upImage.ContentLength;
                if (nFileLen == 0)
                    return "没有选择上传图片";
                //获取upImage选择文件的扩展名
                string extendName = System.IO.Path.GetExtension(upImage.FileName).ToLower();
                //判断是否为图片格式
                if (extendName != ".jpg" && extendName != ".jpge" && extendName != ".gif" && extendName != ".bmp" && extendName != ".png")
                    return "图片格式不正确";

                byte[] myData = new Byte[nFileLen];
                upImage.InputStream.Read(myData, 0, nFileLen);
                sFilename = System.IO.Path.GetFileName(upImage.FileName);
                int file_append = 0;
                //检查当前文件夹下是否有同名图片,有则在文件名+1
                while (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sFilename)))
                {
                    file_append++;
                    sFilename = System.IO.Path.GetFileNameWithoutExtension(upImage.FileName)
                        + file_append.ToString() + extendName;
                }
                System.IO.FileStream newFile
                    = new System.IO.FileStream(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sFilename),
                    System.IO.FileMode.Create, System.IO.FileAccess.Write);
                newFile.Write(myData, 0, myData.Length);
                newFile.Close();
                //以上为上传原图
                try
                {
                    //原图加载
                    using (System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sFilename)))
                    {
                        //原图宽度和高度
                        int width = sourceImage.Width;
                        int height = sourceImage.Height;
                        int smallWidth;
                        int smallHeight;
                        //获取第一张绘制图的大小,(比较 原图的宽/缩略图的宽  和 原图的高/缩略图的高)
                        if (((decimal)width) / height <= ((decimal)intThumbWidth) / intThumbHeight)
                        {
                            smallWidth = intThumbWidth;
                            smallHeight = intThumbWidth * height / width;
                        }
                        else
                        {
                            smallWidth = intThumbHeight * width / height;
                            smallHeight = intThumbHeight;
                        }
                        //判断缩略图在当前文件夹下是否同名称文件存在
                        file_append = 0;
                        sThumbFile = sThumbExtension + System.IO.Path.GetFileNameWithoutExtension(upImage.FileName) + extendName;
                        while (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sThumbFile)))
                        {
                            file_append++;
                            sThumbFile = sThumbExtension + System.IO.Path.GetFileNameWithoutExtension(upImage.FileName) +
                                file_append.ToString() + extendName;
                        }
                        //缩略图保存的绝对路径
                        string smallImagePath = System.Web.HttpContext.Current.Server.MapPath(sSavePath) + sThumbFile;
                        //新建一个图板,以最小等比例压缩大小绘制原图
                        using (System.Drawing.Image bitmap = new System.Drawing.Bitmap(smallWidth, smallHeight))
                        {
                            //绘制中间图
                            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                            {
                                //高清,平滑
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                g.Clear(Color.Black);
                                g.DrawImage(
                                sourceImage,
                                new System.Drawing.Rectangle(0, 0, smallWidth, smallHeight),
                                new System.Drawing.Rectangle(0, 0, width, height),
                                System.Drawing.GraphicsUnit.Pixel
                                );
                            }
                            //新建一个图板,以缩略图大小绘制中间图
                            using (System.Drawing.Image bitmap1 = new System.Drawing.Bitmap(intThumbWidth, intThumbHeight))
                            {
                                //绘制缩略图  http://www.cnblogs.com/sosoft/
                                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap1))
                                {
                                    //高清,平滑
                                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                    g.Clear(Color.Black);
                                    int lwidth = (smallWidth - intThumbWidth) / 2;
                                    int bheight = (smallHeight - intThumbHeight) / 2;
                                    g.DrawImage(bitmap, new Rectangle(0, 0, intThumbWidth, intThumbHeight), lwidth, bheight, intThumbWidth, intThumbHeight, GraphicsUnit.Pixel);
                                    g.Dispose();
                                    bitmap1.Save(smallImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //出错则删除
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(sSavePath + sFilename));
                    return "图片格式不正确";
                }
                //返回缩略图名称
                return sThumbFile;
            }
            return "没有选择图片";
        }


      
    }
}
