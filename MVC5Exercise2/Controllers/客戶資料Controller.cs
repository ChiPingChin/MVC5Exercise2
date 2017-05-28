using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC5Exercise2.Models;
using PagedList;
using MVC5Exercise2.ActionFilters;
using ClosedXML.Excel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using FastMember;

namespace MVC5Exercise2.Controllers
{
    [ActionLog]
    public class 客戶資料Controller : Controller
    {
        private CustomerEntities db = new CustomerEntities();
        // 改用 Repository Pattern 管理所有新刪查改(CRUD)等功能 & 資料篩選的邏輯要寫在 Repository 的類別裡面
        客戶資料Repository repo = RepositoryHelper.Get客戶資料Repository();

        // 設定每一頁筆數
        private int pageSize = 3;

        public ActionResult Index(string sortOrder, string 客戶分類 = "", int page = 1)
        {
            // 測試多國語系 - .Net程式依語系取得資源內容 - 在 Controller/Action 套用多國屬性設定值
            ViewBag.Query = App_GlobalResources.Resource.Query;

            int currentPage = page < 1 ? 1 : page;
            IQueryable<客戶資料> data = null;
            if (string.IsNullOrEmpty(客戶分類))
            {
                data = repo.Get列表所有客戶資料();
            }
            else
            {
                data = repo.Get列表所有客戶資料()
                    .Where(c => c.客戶分類.ToUpper() == 客戶分類.ToUpper());
            }

            // *** 實作排序功能(點選同一欄位切換下一次搜尋條件，回傳給 View 紀錄下來) Begin ***
            // 先實作2個欄位排序：客戶名稱 & 統一編號
            ViewBag.CNameSortParm = String.IsNullOrEmpty(sortOrder) ? "cname_desc" : "";
            ViewBag.CNumberSortParm = sortOrder == "cnumber" ? "cnumber_desc" : "cnumber";

            switch (sortOrder)
            {
                case "cname_desc":
                    data = data.OrderByDescending(c => c.客戶名稱);
                    break;
                case "cnumber":
                    data = data.OrderBy(c => c.統一編號);
                    break;
                case "cnumber_desc":
                    data = data.OrderByDescending(c => c.統一編號);
                    break;
                default:
                    data = data.OrderBy(c => c.客戶名稱);
                    break;
            }
            // *** 實作排序功能(點選同一欄位切換下一次搜尋條件，回傳給 View 紀錄下來) Begin ***

            //return View(data.ToList());
            var result = data.ToPagedList(currentPage, pageSize);
            return View(result);

        }

        /// <summary>
        /// 匯出 Excel File 
        /// http://www.dotnet-tutorial.com/articles/mvc/export-data-in-excel-file-with-asp-net-mvc
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportToExcel()
        {
            var gv = new System.Web.UI.WebControls.GridView();
            gv.DataSource = repo.Get列表所有客戶資料().ToList();
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

            return View("Index");
        }


        /// <summary>
        /// 匯出 Excel File (需先使用 NuGet 新增 ClosedXML + FastMember 兩個套件 - 回傳 FileStreamResult)
        /// http://www.c-sharpcorner.com/UploadFile/rahul4_saxena/export-data-table-to-excel-in-Asp-Net-mvc-4/   ***
        /// https://stackoverflow.com/questions/564366/convert-generic-list-enumerable-to-datatable              FastMember 套件
        /// https://closedxml.codeplex.com/wikipage?title=Showcase&referringTitle=Documentation
        /// https://closedxml.codeplex.com/wikipage?title=Basic%20Table&referringTitle=Documentation
        /// http://blog.darkthread.net/post-2012-12-28-closedxml.aspx
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportToExcelWithClosedXML()
        {
            // 先取得 DB 中資料成為 IEnumerable<T> 型別 (自訂輸出欄位)
            var data = db.客戶資料.Select(c => new
            {
                客戶名稱 = c.客戶名稱,
                客戶分類 = c.客戶分類,
                統一編號 = c.統一編號,
                電話 = c.電話,
                傳真 = c.傳真,
                地址 = c.地址,
                Email = c.Email
            });

            // 建立一個新的 DataTable
            DataTable table = new DataTable();

            // 使用 FastMember 套件的方法將 IEnumerable<T> 資料轉成 DataTable 餵給 ClosedXML 元件，以準備輸出 Excel File
            using (var reader = ObjectReader.Create(data))
            {
                table.Load(reader);
            }

            // 使用 ClosedXML 套件製作 Excel File (把 DataTable 資料餵給第一個 Worksheets)
            XLWorkbook wb = new XLWorkbook();
            // 新增一個頁籤，名叫 Sheet1，並將 DataTable 資料餵給他
            var ws = wb.Worksheets.Add(table, "Sheet1");

            // 格式化資料呈現
            wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            wb.Style.Font.Bold = true;
            // Column Auto Adjust
            ws.Columns().AdjustToContents();

            // Declare file-type(content-type)
            string fileType = @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // 建立一個 MemoryStream 準備放 Excel 內容的輸出
            MemoryStream MyMemoryStream = new MemoryStream();
            wb.SaveAs(MyMemoryStream);
            wb.Dispose();  // 釋放資源
            MyMemoryStream.Position = 0;
            // 將 MemoryStream Excel 檔案內容回傳出檔案(FileStreamResult)
            return File(MyMemoryStream, fileType, "CustomerListExp.xlsx");

            //return RedirectToAction("Index", "客戶資料");
        }


        /// <summary>
        /// 檔案下載作業(僅供參考)
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadFile2()
        {
            //我要下載的檔案位置
            string filePath = Server.MapPath("~/FileUploads/IMG20170424105915.zip");
            string fileType = @"application/zip";

            //取得檔案名稱 (準備給之後下載時的檔案預設命名)
            string fileName = Path.GetFileName(filePath);

            // 讀成串流
            Stream iStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            // 回傳出檔案
            return File(iStream, fileType, fileName);
        }



        /// <summary>
        /// 後端回傳 Json 格式資料
        /// Json Parse 網址：http://json.parser.online.fr/  
        /// </summary>
        /// <returns></returns>
        public ActionResult GetJson()
        {
            repo.UnitOfWork.Context.Configuration.LazyLoadingEnabled = false;
            return Json(repo.Get列表所有客戶資料().Take(3), JsonRequestBehavior.AllowGet);
        }

        // GET: 客戶資料/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //客戶資料 客戶資料 = db.客戶資料.Find(id);
            客戶資料 客戶資料 = repo.Get單筆客戶資料ByClientId(id.Value);

            if (客戶資料 == null)
            {
                return HttpNotFound();
            }
            return View(客戶資料);
        }

        // GET: 客戶資料/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: 客戶資料/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[HandleError(View = "Error")]
        [HandleError(ExceptionType = typeof(Exception), View = "Error")]
        // 參考：Exception or Error Handling in ASP.Net MVC Using HandleError Attribute
        // http://www.c-sharpcorner.com/UploadFile/ff2f08/exception-or-error-handling-in-Asp-Net-mvc-using-handleerror/
        public ActionResult Create([Bind(Include = "Id,客戶名稱,統一編號,電話,傳真,地址,Email,是否已刪除,客戶分類")] 客戶資料 客戶資料)
        {
            // throw new Exception("故意製造一個錯誤，請忽略!!!");

            if (ModelState.IsValid)
            {
                //db.客戶資料.Add(客戶資料);
                //db.SaveChanges();

                repo.Add(客戶資料);
                repo.UnitOfWork.Commit();

                return RedirectToAction("Index");
            }

            return View(客戶資料);
        }

        // GET: 客戶資料/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //客戶資料 客戶資料 = db.客戶資料.Find(id);
            客戶資料 客戶資料 = repo.Get單筆客戶資料ByClientId(id.Value);

            if (客戶資料 == null)
            {
                return HttpNotFound();
            }
            return View(客戶資料);
        }

        // POST: 客戶資料/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // public ActionResult Edit([Bind(Include = "Id,客戶名稱,統一編號,電話,傳真,地址,Email,是否已刪除,客戶分類")] 客戶資料 客戶資料)
        public ActionResult Edit(int id, FormCollection form)
        {
            客戶資料 客戶資料 = repo.Get單筆客戶資料ByClientId(id);
            if (TryUpdateModel(客戶資料, new string[] { "密碼", "電話", "傳真", "地址", "Email" }))
            {
                repo.UnitOfWork.Commit();

                return RedirectToAction("Index");
            }
            return View(客戶資料);
        }

        // GET: 客戶資料/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //客戶資料 客戶資料 = db.客戶資料.Find(id);
            客戶資料 客戶資料 = repo.Get單筆客戶資料ByClientId(id.Value);

            if (客戶資料 == null)
            {
                return HttpNotFound();
            }
            return View(客戶資料);
        }

        // POST: 客戶資料/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //客戶資料 客戶資料 = db.客戶資料.Find(id);
            客戶資料 客戶資料 = repo.Get單筆客戶資料ByClientId(id);

            //db.客戶資料.Remove(客戶資料);
            //db.SaveChanges();
            repo.Delete(客戶資料);
            repo.UnitOfWork.Commit();

            return RedirectToAction("Index");
        }




    }
}


/// 排序、搜尋與分頁(後端作法參考網址)
// https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
// http://www.aizhengli.com/entity-framework6-mvc5-started/111/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application.html
// 排序、搜尋與分頁(前端作法參考網址) ASP.NET MVC 使用 jQuery EasyUI DataGrid -排序(Sorting)
// http://kevintsengtw.blogspot.tw/2013/10/aspnet-mvc-jquery-easyui-datagrid_9.html
// http://kevintsengtw.blogspot.tw/2013/10/aspnet-mvc-jquery-easyui-datagrid_10.html
// ASP.NET MVC 資料分頁 - 使用 PagedList.Mvc *
// http://kevintsengtw.blogspot.tw/2013/10/aspnet-mvc-pagedlistmvc.html
// https://forums.asp.net/t/1825413.aspx?PagedList
// ASP.NET MVC 資料分頁 - 使用 PagedList.Mvc：分頁列樣式
// http://kevintsengtw.blogspot.tw/2013/10/aspnet-mvc-pagedlistmvc_17.html
// ASP.NET MVC 資料分頁操作 - 使用 PagedList.Mvc ＠ GitHub  (All)
// http://kevintsengtw.blogspot.tw/2014/10/aspnet-mvc-pagedlistmvc-github.html