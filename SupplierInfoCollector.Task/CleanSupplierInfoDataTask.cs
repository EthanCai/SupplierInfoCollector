using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nana.Framework.Utility;
using SupplierInfoCollector.Business;
using SupplierInfoCollector.Domain;
using SupplierInfoCollector.TaskFramework;

namespace SupplierInfoCollector.Task
{
    public class CleanSupplierInfoDataTask : ITask
    {
        public TastExecuteResult Execute()
        {
            SupplierInfoBusiness business = new SupplierInfoBusiness();

            List<SupplierInfo> supplierInfoList = business.GetSupplierInfoListFromDB();

            foreach (var supplierInfo in supplierInfoList)
            {
                try
                {
                    supplierInfo.Name = CleanText(supplierInfo.Name);
                    supplierInfo.PhoneNumber = CleanText(supplierInfo.PhoneNumber);
                    supplierInfo.FaxNumber = CleanText(supplierInfo.FaxNumber);
                    supplierInfo.Contactor = CleanText(supplierInfo.Contactor);
                    supplierInfo.ContactorTitle = CleanText(supplierInfo.ContactorTitle);
                    supplierInfo.Address = CleanText(supplierInfo.Address);
                    supplierInfo.City = CleanText(supplierInfo.City);
                    supplierInfo.Country = CleanText(supplierInfo.Country);
                    supplierInfo.StateOrProvince = CleanText(supplierInfo.StateOrProvince);
                    supplierInfo.PostCode = CleanText(supplierInfo.PostCode);

                    business.UpdateSupplierInfo(supplierInfo);

                    Console.WriteLine(string.Format("完成清理{0} {1}", supplierInfo.Id, supplierInfo.Name));
                }
                catch (Exception ex)
                {
                    string message = string.Format("清理失败{0} {1}\r\nmessage:{2}\r\nstacktrace:{3}",
                        supplierInfo.Id, supplierInfo.Name, ex.Message, ex.StackTrace);
                    Console.WriteLine(message);
                    TxtLogHelper.Instance.Log(message, EnumConfigLogLevel.Error);
                }
            }

            return new TastExecuteResult()
            {
                Result = 0,
                Message = "清理供应商详细信息" + supplierInfoList.Count + "个"
            };
        }

        private string CleanText(string text)
        {
            if (text == null)
            {
                return null;
            }
            
            string invalidText1 = Encoding.UTF8.GetString(new byte[] {0xC2, 0xA0});
            bool b1 = text.Contains(invalidText1);

            return text.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("&nbsp;", " ").Replace(invalidText1, " ");
        }
    }
}
