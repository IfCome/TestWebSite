using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrowdFundingShop.UI.Controllers.PC
{
    public class UserInfoController : PCBaseController
    {
        //
        // GET: /UserInfo/

        public ActionResult AddUser()
        {
            return View();
        }

        public ActionResult IndexPage()
        {
            return View();
        }
    }
}
