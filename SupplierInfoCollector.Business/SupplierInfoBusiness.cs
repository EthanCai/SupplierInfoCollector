using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupplierInfoCollector.DataAccess;
using SupplierInfoCollector.Domain;
using SupplierInfoCollector.WebPageUtils;

namespace SupplierInfoCollector.Business
{
    public class SupplierInfoBusiness : BaseBusiness
    {
        private readonly SupplierInfoCrawler crawler = new SupplierInfoCrawler();
        private readonly SupplierInfoMongoDBDataAccess dataAccess = new SupplierInfoMongoDBDataAccess();

        public List<SupplierInfo> GetSupplierInfoListFromWeb(List<string> listPageURLTemplateList)
        {
            return crawler.GetSupplierInfoList(listPageURLTemplateList);
        }

        public SupplierInfo GetSupplierDetailsFromWeb(SupplierInfo supplierInfo)
        {
            return crawler.GetSupplierDetails(supplierInfo);
        }

        public List<SupplierInfo> GetSupplierInfoListFromDB()
        {
            return dataAccess.GetSupplierInfoList();
        }

        public void InsertSupplierInfo(SupplierInfo supplierInfo)
        {
            dataAccess.InsertSupplierInfo(supplierInfo);
        }

        public void UpdateSupplierInfo(SupplierInfo supplierInfo)
        {
            dataAccess.UpdateSupplierInfo(supplierInfo);
        }

        public SupplierInfo GetSupplierInfo(string globalSoucesId)
        {
            return dataAccess.GetSupplierInfo(globalSoucesId);
        }
    }
}
