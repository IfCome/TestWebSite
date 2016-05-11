using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CrowdFundingShop.Utility
{
    public class Config
    {
        public static string AppID
        {
            get { return ConfigurationManager.AppSettings["AppID"]; }
        }

        public static string AppSecret
        {
            get { return ConfigurationManager.AppSettings["AppSecret"]; }
        }

        public static string DomainUrl
        {
            get { return ConfigurationManager.AppSettings["domainurl"]; }
        }

        public static string OauthUrl
        {
            get { return ConfigurationManager.AppSettings["oauth_url"]; }
        }
    }
}
