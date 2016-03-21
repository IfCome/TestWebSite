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
        private String _uploadImageName = String.Empty; // 上传图片保存名称
        private String _thumbImageName = String.Empty;  // 缩略图保存名称

        private String _uploadImageUrl = String.Empty;  // 上传图片下载路径
        private String _thumbImageUrl = String.Empty;   // 缩略图下载路径

        private bool _isSuccess = false;    // 是否上传成功
        private String _errorInfo = String.Empty;   // 错误信息

        private int _intThumbWidth = 0; // 缩略图宽
        private int _intThumbHeight = 0;    // 缩略图高

        private String _uploadPath = String.Empty;  // 图片保存物理文件夹路径
        private String _serverPath = String.Empty;  // 图片下载url文件夹路径

        /// <summary>
        /// 原图地址
        /// </summary>
        public String UploadImageUrl { get { return _uploadImageUrl; } }

        /// <summary>
        /// 缩略图地址
        /// </summary>
        public String ThumbImageUrl { get { return _thumbImageUrl; } }

        /// <summary>
        /// 是否上传成功
        /// </summary>
        public bool IsSuccess { get { return _isSuccess; } }

        /// <summary>
        /// 上传失败，返回错误信息
        /// </summary>
        public String ErrorInfo { get { return _errorInfo; } }


        /// <summary>
        /// asp.net上传图片并生成缩略图
        /// </summary>
        /// <param name="upImage">HttpPostedFileBase控件</param>
        /// <param name="sSavePath">保存的路径,些为相对服务器路径的下的文件夹</param>
        /// <param name="sThumbExtension">缩略图的thumb</param>
        /// <param name="intThumbWidth">生成缩略图的宽度</param>
        /// <param name="intThumbHeight">生成缩略图的高度</param>
        public FileUpToImg(HttpPostedFileBase upImage, string sSavePath, string sThumbExtension = "_t", int intThumbWidth = 150, int intThumbHeight = 150)
        {
            if (upImage == null || upImage.ContentLength == 0)
            {
                _isSuccess = false;
                _errorInfo = "没有选择上传图片";
                return;
            }

            //获取upImage选择文件的扩展名
            string extendName = System.IO.Path.GetExtension(upImage.FileName).ToLower();
            //判断是否为图片格式
            if (extendName != ".jpg" && extendName != ".jpge" && extendName != ".gif" && extendName != ".bmp" && extendName != ".png")
            {
                _isSuccess = false;
                _errorInfo = "图片格式不正确";
                return;
            }

            // 文件名称
            string dateNowStr = DateTime.Now.ToString("yyyyMMddhhmmssmmm");
            _uploadImageName = dateNowStr + extendName;
            _thumbImageName = dateNowStr + (sThumbExtension.IsNullOrWhiteSpace() ? "_t" : sThumbExtension) + extendName;

            // 缩略图大小
            _intThumbHeight = intThumbHeight;
            _intThumbWidth = intThumbWidth;

            // 获取图片保存的物理文件夹路径
            _uploadPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/"), "images", sSavePath);
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);

            // 获取图片下载的url虚拟文件夹路径
            _serverPath = Path.Combine("http://", HttpContext.Current.Request.Url.Host.ToString(), "images", sSavePath);

            UpLoadImage(upImage);
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
        public void UpLoadImage(HttpPostedFileBase upImage)
        {
            int nFileLen = upImage.ContentLength;
            byte[] myData = new Byte[nFileLen];
            upImage.InputStream.Read(myData, 0, nFileLen);

            System.IO.FileStream newFile
                = new System.IO.FileStream(Path.Combine(this._uploadPath, this._uploadImageName),
                System.IO.FileMode.Create, System.IO.FileAccess.Write);
            newFile.Write(myData, 0, myData.Length);
            newFile.Close();
            //以上为上传原图
            try
            {
                //原图加载
                using (System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(Path.Combine(this._uploadPath, this._uploadImageName)))
                {
                    //原图宽度和高度
                    int width = sourceImage.Width;
                    int height = sourceImage.Height;
                    int smallWidth;
                    int smallHeight;
                    //获取第一张绘制图的大小,(比较 原图的宽/缩略图的宽  和 原图的高/缩略图的高)
                    if (((decimal)width) / height <= ((decimal)_intThumbWidth) / _intThumbHeight)
                    {
                        smallWidth = _intThumbWidth;
                        smallHeight = _intThumbWidth * height / width;
                    }
                    else
                    {
                        smallWidth = _intThumbHeight * width / height;
                        smallHeight = _intThumbHeight;
                    }

                    // 缩略图保存的绝对路径
                    string smallImagePath = Path.Combine(this._uploadPath, this._thumbImageName);
                    // 新建一个图板,以最小等比例压缩大小绘制原图
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
                        using (System.Drawing.Image bitmap1 = new System.Drawing.Bitmap(_intThumbWidth, _intThumbHeight))
                        {
                            //绘制缩略图
                            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap1))
                            {
                                //高清,平滑
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                g.Clear(Color.Black);
                                int lwidth = (smallWidth - _intThumbWidth) / 2;
                                int bheight = (smallHeight - _intThumbHeight) / 2;
                                g.DrawImage(bitmap, new Rectangle(0, 0, _intThumbWidth, _intThumbHeight), lwidth, bheight, _intThumbWidth, _intThumbHeight, GraphicsUnit.Pixel);
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
                System.IO.File.Delete(Path.Combine(this._uploadPath, this._uploadImageName));
                _isSuccess = false;
                _errorInfo = "图片格式不正确";
            }
            _isSuccess = true;
            _uploadImageUrl = Path.Combine(this._serverPath, this._uploadImageName);
            _thumbImageUrl = Path.Combine(this._serverPath, this._thumbImageName);
        }

    }
}
