using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

namespace MVC5Exercise2.Models
{   
	public  class 客戶資料Repository : EFRepository<客戶資料>, I客戶資料Repository
	{
        public override IQueryable<客戶資料> All()
        {
            return base.All().Where(c => !c.是否已刪除);  
        }

        public 客戶資料 Get單筆客戶資料ByClientId(int id)
        {
            return this.All().FirstOrDefault(c => c.Id == id);
        }

        public IQueryable<客戶資料> Get列表所有客戶資料(bool showAll = false)
        {
            IQueryable<客戶資料> all = this.All();

            if (!showAll)
            {
                all = this.All().Take(10);
            }

            return all;
        }

        public void Update(客戶資料 customer)
        {
            this.UnitOfWork.Context.Entry(customer).State = EntityState.Modified;
        }

        public override void Delete(客戶資料 entity)
        {
            this.UnitOfWork.Context.Configuration.ValidateOnSaveEnabled = false;
            entity.是否已刪除 = true;
        }



    }

	public  interface I客戶資料Repository : IRepository<客戶資料>
	{

	}
}