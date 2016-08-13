using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SupplierInfoCollector.Business;
using SupplierInfoCollector.Domain;
using SupplierInfoCollector.TaskFramework;

namespace SupplierInfoCollector.Task
{
    public class FetchSupplierListTask : ITask
    {
        public TastExecuteResult Execute()
        {
            SupplierInfoBusiness business = new SupplierInfoBusiness();

            List<SupplierInfo> list = business.GetSupplierInfoListFromWeb(new List<string>()
            {
                "http://www.globalsources.com/gsol/I/Furniture-Furnishing-suppliers/s/2000000003844/3000000150838/-1/{0}.htm",
                "http://www.globalsources.com/gsol/I/Hardware-DIY-suppliers/s/2000000003844/3000000152204/-1/{0}.htm"
            });

            foreach (var item in list)
            {
                var itemInDB = business.GetSupplierInfo(item.GlobalSourcesId);

                if (itemInDB == null)
                {
                    business.InsertSupplierInfo(item);
                }
                else
                {

                    itemInDB.Name = item.Name;
                    itemInDB.Ranking = item.Ranking;
                    itemInDB.YearsSince = item.YearsSince;
                    itemInDB.ProductCount = item.ProductCount;
                    itemInDB.GlobalSourcesHomePageURL = item.GlobalSourcesHomePageURL;
                    itemInDB.FullCatalogPageURL = item.FullCatalogPageURL;
                    itemInDB.AddSupplierType(item.SupplierType);
                    business.UpdateSupplierInfo(itemInDB);
                }
            }

            return new TastExecuteResult()
            {
                Result = 0,
                Message = "抓取供应商列表信息" + list.Count + "个"
            };
        }
    }
}
