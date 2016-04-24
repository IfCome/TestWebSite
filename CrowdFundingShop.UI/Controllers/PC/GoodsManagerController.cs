using CrowdFundingShop.UI.Models.PC;
using CrowdFundingShop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrowdFundingShop.UI.Controllers.PC
{
    public class GoodsManagerController : PCBaseController
    {
        //
        // GET: /GoodsManager/

        public ActionResult IndexPage()
        {
            return View();
        }

        public ActionResult GetGoodsList(int pageSize, int currentPage, int category = 0, string keyWords = "", string huoDongState = "")
        {
            List<Model.GoodsBaseInfo> goodsInfoList = new List<Model.GoodsBaseInfo>();
            int allCount = 0;
            goodsInfoList = BLL.GoodsBaseInfoBll.GetList(pageSize, currentPage, keyWords, category, (huoDongState == "-1" ? "" : huoDongState), out allCount);
            if (goodsInfoList != null)
            {
                return Json(new
                {
                    Rows = goodsInfoList.Select(g => new
                    {
                        g.ID,
                        g.GoodsName,
                        g.DetailIcons,
                        Describe = (g.Describe.Length > 50 ? (g.Describe.Remove(50, g.Describe.Length - 50) + "......") : g.Describe),
                        g.Price,
                        g.ShowIcons,
                        g.Category,
                        g.CreateTime,
                        g.State,
                        g.ZhongChouCount,
                        ZhongChouPercent = (g.Price != "0" && g.Price != "" && g.ZhongChouCount != 0) ? ((g.ZhongChouCount * 100.0 / Converter.TryToInt32(g.Price)) < 1 ? 2 : (g.ZhongChouCount * 100.0 / Converter.TryToInt32(g.Price))) : 0
                    }),
                    AllCount = allCount
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Rows = "", AllCount = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit()
        {
            return View();
        }
        //添加商品信息
        public ActionResult AddGoodsCallBack(AddGoodsCallBackIn InModel)
        {
            string errorType = "";
            string msg = "OK";
            //用来返回刚插入的记录的ID
            string insertID = "";
            // 验证参数
            if (string.IsNullOrWhiteSpace(InModel.GoodsName))
            {
                errorType = "GoodsName";
                msg = "请输入商品名称";
            }
            else if (BLL.BackgroundUserBll.IsExistUserName(InModel.Describe))
            {
                errorType = "Describe";
                msg = "请输入商品详情";
            }

            else if (string.IsNullOrWhiteSpace(InModel.Price))
            {
                errorType = "Price";
                msg = "请输入商品价格";
            }
            else if (string.IsNullOrWhiteSpace(InModel.Category))
            {
                errorType = "Category";
                msg = "请选择商品类别";
            }
            else
            {
                // 添加用户
                Model.GoodsBaseInfo goodsInfo = new Model.GoodsBaseInfo()
                {
                    GoodsName = InModel.GoodsName,
                    Price = InModel.Price,
                    Describe = InModel.Describe,
                    Category = InModel.Category,
                    CreateUserID = Identity.LoginUserInfo.ID.ToString(),
                    CreateTime = DateTime.Now.ToString()
                };
                insertID = BLL.GoodsBaseInfoBll.AddGoodsInfo(goodsInfo);
                if (string.IsNullOrEmpty(insertID))
                {
                    errorType = "alert";
                    msg = "添加失败，请重试";
                }
            }
            return Json(new { Message = msg, ErrorType = errorType, InsertID = insertID }, JsonRequestBehavior.AllowGet);
        }
        //添加商品图片信息（更新）
        public ActionResult EditPic(string id)
        {
            ViewBag.ID = id;
            if (string.IsNullOrEmpty(id))
            {
                Response.Redirect("/GoodsManager/Edit");
            }
            return View();
        }
        //查商品分类
        public ActionResult GetCategoryInfo(int parentID)
        {
            List<Model.CategoryInfo> categoryInfoList = new List<Model.CategoryInfo>();
            categoryInfoList = BLL.CategoryInfoBll.GetListByParentID(parentID);
            if (categoryInfoList != null)
            {
                return Json(new
                {
                    Rows = categoryInfoList.Select(g => new
                    {
                        ID = g.ID,
                        ParentID = g.ParentId,
                        CategoryName = g.CategoryName
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        //更新商品信息
        public ActionResult UpdateGoodsCallBack(AddGoodsCallBackIn InModel)
        {
            string errorType = "";
            string msg = "OK";
            Model.GoodsBaseInfo goodsInfo = new Model.GoodsBaseInfo()
               {
                   ID = InModel.ID,
                   GoodsName = InModel.GoodsName,
                   Price = InModel.Price,
                   Describe = InModel.Describe,
                   Category = InModel.Category,
                   ShowIcons = string.IsNullOrEmpty(InModel.ShowIcons) ? "" : InModel.ShowIcons.Remove(InModel.ShowIcons.Length - 1),
                   DetailIcons = string.IsNullOrEmpty(InModel.DetailIcons) ? "" : InModel.DetailIcons.Remove(InModel.DetailIcons.Length - 1),
                   CreateUserID = Identity.LoginUserInfo.ID.ToString(),
                   CreateTime = DateTime.Now.ToString()
               };
            bool result = BLL.GoodsBaseInfoBll.UpDateGoodsInfo(goodsInfo);
            if (!result)
            {
                errorType = "alert";
                msg = "添加失败，请重试";
            }
            return Json(new { Message = msg, ErrorType = errorType }, JsonRequestBehavior.AllowGet);
        }
        //增加到众筹
        public ActionResult AddHuoDong(AddHuoDongInfoIn InModel)
        {
            string errorType = "";
            string msg = "OK";
            Model.HuoDongInfo huodongInfo = new Model.HuoDongInfo()
            {
                GoodsID = Converter.TryToInt64(InModel.GoodsID),
                FinishedTime = Converter.TryToDateTime(InModel.FinishedTime),
                ShareCount = Converter.TryToInt32(InModel.ShareCount),
                State = 10,//开始众筹
                CreateTime = DateTime.Now,
                CreateUser = Identity.LoginUserInfo.ID.ToString(),
            };
            bool result = BLL.HuoDongInfoBll.Add(huodongInfo);
            if (!result)
            {
                errorType = "alert";
                msg = "添加失败，请重试";
            }
            return Json(new { Message = msg, ErrorType = errorType }, JsonRequestBehavior.AllowGet);
        }
        //编辑商品页面
        public ActionResult EditGoodsInfo(string goodsID)
        {
            Model.GoodsBaseInfo outModel = BLL.GoodsBaseInfoBll.GetGoodsInfoByID(goodsID);
            return View(outModel);
        }
        //查看商品页面
        public ActionResult GetGoodsInfo(string goodsID)
        {
            Model.GoodsBaseInfo outModel = BLL.GoodsBaseInfoBll.GetGoodsInfoByID(goodsID);
            return View(outModel);
        }
        public ActionResult DeleteGoodsInfo(string id)
        {
            string errorType = "";
            string msg = "OK";
            bool result = BLL.GoodsBaseInfoBll.DeleteGoodsInfo(id);
            if (!result)
            {
                errorType = "alert";
                msg = "删除失败，请重试";
            }
            return Json(new { Message = msg, ErrorType = errorType }, JsonRequestBehavior.AllowGet);
        }
    }
}
