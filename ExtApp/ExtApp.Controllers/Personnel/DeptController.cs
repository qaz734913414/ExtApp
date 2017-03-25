﻿using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

using ExtApp.Model;
using ExtApp.BLL;
using NHibernate.Criterion;

namespace ExtApp.Controller
{
    /// <summary>
    /// 组织机构控制器
    /// </summary>
    public class DeptController : ApiBase
    {
        /// <summary>
        /// bll
        /// </summary>
        private DeptBLL bll;

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult List()
        {
            var list = bll.List().Select(o => new
            {
                ID = o.ID,
                PID = o.PDept == null ? 0 : o.PDept.ID,
                PName = o.PDept == null ? "" : o.PDept.Name,
                Code = o.Code,
                Name = o.Name,
                Type = o.Type,
                AddUserID = o.AddUser == null ? 0 : o.AddUser.ID,
                AddUserName = o.AddUser == null ? "" : o.AddUser.Name,
                AddTime = o.AddTime == null ? "" : o.AddTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                Sort = o.Sort,
                Status = o.Status,
                Comment = o.Comment
            }).ToList();

            return Json(list);
        }

        /// <summary>
        /// 获取所有子部门
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public JsonResult GetChildNodes(int PID)
        {
            var list = bll.GetChildNodes(PID);
            return base.List<DeptTreeNode>(list.Count(), list);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Add([FromBody]DeptEditModel model)
        {
            var dept = new Dept
            {
                ID = 0,
                PDept = new Dept { ID = model.PID },
                Code = "",
                Name = model.Name,
                Type = model.Type == null ? 0 : model.Type.Value,
                AddUser = AdminHelper.Admin,
                AddTime = DateTime.Now,
                Sort = model.Sort == null ? 0 : model.Sort.Value,
                Status = model.Status == null ? 1 : model.Status.Value,
                Comment = model.Comment
            };
            return Json(bll.Add(dept));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(DeptEditModel model)
        {
            // 获取机构
            var dept = bll.Get(model.ID);
            if (dept == null)
            {
                return Error("机构不存在！");
            }

            dept.Comment = model.Comment;
            dept.Name = model.Name;
            dept.PDept = model.PID == 0 ? null : new Dept { ID = model.PID };
            dept.Sort = model.Sort == null ? 0 : model.Sort.Value;
            dept.Status = model.Status == null ? 1 : model.Status.Value;
            dept.Type = model.Type == null ? 0 : model.Type.Value;

            return Json(bll.Edit(dept));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var result = bll.Delete(id);
            if (result)
            {
                return base.Success("删除成功");
            }
            else
            {
                return base.Success("删除失败");
            }
        }
    }
}
