using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrowdFundingShop.Utility;

namespace CrowdFundingShop.UI.Controllers.PC
{
    public class ConsumerInfoController : PCBaseController
    {
        //
        // GET: /ConsumerInfo/

        public ActionResult ListPage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetConsumerInfoList(Models.PC.GetConsumerInfoListIn InModel)
        {
            InModel.PageSize = Converter.TryToInt32(InModel.PageSize, 15);
            InModel.CurrentPage = Converter.TryToInt32(InModel.CurrentPage, 1);
            InModel.KeyWords = InModel.KeyWords ?? "";

            List<Model.ConsumerInfo> consumerInfoList = new List<Model.ConsumerInfo>();
            int allCount = 0;
            consumerInfoList = BLL.ConsumerInfoBll.GetAllConsumerInfoList((int)InModel.PageSize, (int)InModel.CurrentPage, InModel.KeyWords, out allCount);
            return Json(new
            {
                Rows = consumerInfoList.Select(u => new
                {
                    u.ID,
                    u.WeiXinAccount,
                    u.NickName,
                    u.Phone,
                    u.HeadIcon,
                    u.JiJiangJieXiao,
                    u.YiJieXiao,
                    u.CartCount,
                    u.Address,
                    u.PostCode
                }),
                AllCount = allCount
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
