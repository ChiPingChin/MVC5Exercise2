using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC5Exercise2.Models;

namespace MVC5Exercise2.Controllers
{
    public class 客戶資料Controller : Controller
    {
        //private CustomerEntities db = new CustomerEntities();
        客戶資料Repository repo = RepositoryHelper.Get客戶資料Repository();

        // GET: 客戶資料
        public ActionResult Index(string 客戶分類 = "")
        {
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
            return View(data);
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
        public ActionResult Create([Bind(Include = "Id,客戶名稱,統一編號,電話,傳真,地址,Email,是否已刪除,客戶分類")] 客戶資料 客戶資料)
        {
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
            if (TryUpdateModel(客戶資料,new string[] { "客戶名稱", "統一編號", "電話", "傳真", "地址", "Email", "客戶分類" }))
            {
                // db.Entry(客戶資料).State = EntityState.Modified;
                // db.SaveChanges();
                
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
            客戶資料 客戶資料 = repo.Get單筆客戶資料ByClientId(id.Value);

            //db.客戶資料.Remove(客戶資料);
            //db.SaveChanges();
            repo.Delete(客戶資料);
            repo.UnitOfWork.Commit();

            return RedirectToAction("Index");
        }




    }
}
