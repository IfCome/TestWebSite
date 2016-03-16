using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Utility
{
    public class Config
    {
        public static string ImagePath
        {
            get { return ConfigurationManager.AppSettings["ImagePath"]; }
        }

        public static string FtpUser
        {
            get { return ConfigurationManager.AppSettings["FtpUser"]; }
        }
        public static string FtpPassword
        {
            get { return ConfigurationManager.AppSettings["FtpPassword"]; }
        }


    }
}
