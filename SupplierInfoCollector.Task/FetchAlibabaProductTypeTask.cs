using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupplierInfoCollector.Business;
using SupplierInfoCollector.Domain;
using SupplierInfoCollector.TaskFramework;

namespace SupplierInfoCollector.Task
{
    public class FetchAlibabaProductTypeTask : ITask
    {
        public TastExecuteResult Execute()
        {
            AlibabaProductTypeBusiness business = new AlibabaProductTypeBusiness();

            List<AlibabaProductType> list = business.GetProductTypeListFromWeb();

            foreach (var item in list)
            {
                var itemInMongo = business.GetProductType(item.Id);

                if (itemInMongo == null)
                {
                    business.InsertProductType(item);
                }
                else
                {
                    business.UpdateProductType(item);
                }
            }

            return new TastExecuteResult()
            {
                Result = 0,
                Message = "抓取http://www.1688.com/#jiaju下的产品类型" + list.Count + "个"
            };
        }
    }
}
