using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nana.Framework.MultiThreading;
using SupplierInfoCollector.Business;
using SupplierInfoCollector.Domain;
using SupplierInfoCollector.TaskFramework;

namespace SupplierInfoCollector.Task
{
    public class FetchAlibabaSuppplierListTask : ITask
    {
        private object _lock = new object();

        public TastExecuteResult Execute()
        {
            AlibabaSupplierInfoBusiness business = new AlibabaSupplierInfoBusiness();
            AlibabaProductTypeBusiness business2 = new AlibabaProductTypeBusiness();

            List<AlibabaProductType> typeList = business2.GetProductTypeListFromDB();

            int insertCount = 0;
            int updateCount = 0;

            foreach (var type in typeList)
            {
                List<AlibabaSupplierInfo> list = business.GetSupplierInfoListFromWeb(type);

                foreach (var item in list)
                {
                    var itemInDB = business.GetSupplierInfo(item.Id);

                    if (itemInDB == null)
                    {
                        business.InsertSupplierInfo(item);
                        insertCount++;
                    }
                    else
                    {
                        itemInDB.AddProductType(type);
                        business.UpdateSupplierInfo(itemInDB);
                        updateCount++;
                    }

                }

                Console.WriteLine(string.Format("=========完成抓取{0}的概要信息，已插入{1}，已更新{2}=========",
                    type.Name, insertCount, updateCount));
            }

            List<AlibabaSupplierInfo> list2 = business.GetSupplierInfoListFromDB();

            ////抓取详细信息
            //for (int i = 0; i < list2.Count; i++)
            //{
            //    var item = list2[i];

            //    var supplierInfo = business.GetSupplierDetailsFromWeb(item);

            //    business.UpdateSupplierInfo(supplierInfo);

            //    Console.WriteLine(string.Format("完成抓取供应商信息进度{0}/{1}", i, list2.Count));
            //}

            /* 多线程抓取供应商信息，会导致连接失败，阿里巴巴有防抓取机制

            list2 = list2.Skip(11400).ToList();
            int completed = 0;

            ThreadWorker<AlibabaSupplierInfo> threadWorker =
                     new ThreadWorker<AlibabaSupplierInfo>(list2, 2, item =>
                     {
                         var supplierInfo = business.GetSupplierDetailsFromWeb(item);

                         business.UpdateSupplierInfo(supplierInfo);

                         lock (_lock)
                         {
                             completed++;
                         }

                         Console.WriteLine(string.Format("完成抓取供应商信息进度{0}/{1}", completed, list2.Count));

                     });
            threadWorker.Do();
            */

            int totalCount = list2.Count;

            return new TastExecuteResult()
            {
                Result = 0,
                Message = "保存Alibaba供应商概要信息" + totalCount + "个"
            };
        }
    }
}
