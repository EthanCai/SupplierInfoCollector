using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SupplierInfoCollector.Business;
using SupplierInfoCollector.Domain;
using SupplierInfoCollector.TaskFramework;
using Nana.Framework.Utility;

namespace SupplierInfoCollector.Task
{
    public class FetchSupplierInfoTask : ITask
    {
        public TastExecuteResult Execute()
        {
            SupplierInfoBusiness business = new SupplierInfoBusiness();

            List<SupplierInfo> supplierInfoList = business.GetSupplierInfoListFromDB();

            foreach (var supplierInfo in supplierInfoList)
            {
                try
                {
                    var supplierInfoDetails = business.GetSupplierDetailsFromWeb(supplierInfo);
                    business.UpdateSupplierInfo(supplierInfoDetails);
                    Console.WriteLine(string.Format("完成更新{0} {1}", supplierInfo.Id, supplierInfo.Name));
                }
                catch (Exception ex)
                {
                    string message = string.Format("更新失败{0} {1}\r\nmessage:{2}\r\nstacktrace:{3}",
                        supplierInfo.Id, supplierInfo.Name, ex.Message, ex.StackTrace);
                    Console.WriteLine(message);
                    TxtLogHelper.Instance.Log(message, EnumConfigLogLevel.Error);
                }
            }

            return new TastExecuteResult()
            {
                Result = 0,
                Message = "更新供应商详细信息" + supplierInfoList.Count + "个"
            };
        }
    }
}
