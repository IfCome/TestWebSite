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

        public ActionResult GetGoodsList(int currentPage, string keyWords = "")
        {
            List<Model.GoodsBaseInfo> goodsInfoList = new List<Model.GoodsBaseInfo>();
            int allCount = 0;
            goodsInfoList = BLL.GoodsBaseInfoBll.GetList(currentPage, keyWords, out allCount);
            return Json(new
            {
                Rows = goodsInfoList.Select(g => new
                {
                    g.ID,
                    g.GoodsName,
                    g.DetailIcons,
                    g.Describe,
                    g.Price,
                    g.ShowIcons,
                    g.Category,
                    g.CreateTime,
                }),
                AllCount = allCount
            }, JsonRequestBehavior.AllowGet);
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

        public ActionResult EditPic(string id)
        {
            ViewBag.ID = id;
            if (string.IsNullOrEmpty(id))
            {
                Response.Redirect("/GoodsManager/Edit");
            }
            return View();
        }

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
    }
}
