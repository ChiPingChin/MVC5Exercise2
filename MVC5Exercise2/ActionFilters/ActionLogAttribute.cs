using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5Exercise2.ActionFilters
{
    /// <summary>
    /// 自訂一個動作過濾器，可以記錄每個 Action 與 ActionResult 執行的時間，資料可以輸出到 Debug 輸出視窗
    /// https://github.com/doggy8088/MVC5Course/commit/55183d57fa7554d8adbfdaaf54a2531e48248ccf
    /// http://blog.kkbruce.net/2011/11/aspnet-mvc-log.html#.WR-p-uuGOM8
    /// </summary>
    public class ActionLogAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           // base.OnActionExecuting(filterContext);
            Debug.WriteLine(filterContext.RouteData.Values["controller"] + "/" + filterContext.RouteData.Values["action"] + " OnActionExecuting ...");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //base.OnActionExecuted(filterContext);
            Debug.WriteLine(filterContext.RouteData.Values["controller"] + "/" + filterContext.RouteData.Values["action"] + " OnActionExecuted.");
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //base.OnResultExecuting(filterContext);
            Debug.WriteLine(filterContext.RouteData.Values["controller"] + "/" + filterContext.RouteData.Values["action"] + " OnResultExecuting ...");
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //base.OnResultExecuted(filterContext);
            Debug.WriteLine(filterContext.RouteData.Values["controller"] + "/" + filterContext.RouteData.Values["action"] + " OnResultExecuted.");
        }


    }
}