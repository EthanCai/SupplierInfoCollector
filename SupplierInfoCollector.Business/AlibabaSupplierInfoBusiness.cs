using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupplierInfoCollector.DataAccess;
using SupplierInfoCollector.Domain;
using SupplierInfoCollector.WebPageUtils;

namespace SupplierInfoCollector.Business
{
    public class AlibabaSupplierInfoBusiness : BaseBusiness
    {
        private readonly AlibabaSupplierInfoCrawler _crawler = new AlibabaSupplierInfoCrawler();
        private readonly AlibabaSupplierInfoMongoDBDataAccess _supplierInfoDataAccess = 
            new AlibabaSupplierInfoMongoDBDataAccess();

        #region AlibabaSupplierInfo

        public List<AlibabaSupplierInfo> GetSupplierInfoListFromWeb(AlibabaProductType productType)
        {
            return _crawler.GetAlibabaSupplierInfoList(productType);
        }

        public AlibabaSupplierInfo GetSupplierDetailsFromWeb(AlibabaSupplierInfo supplierInfo)
        {
            return _crawler.GetSupplierDetailsFromWeb(supplierInfo);
        }

        public List<AlibabaSupplierInfo> GetSupplierInfoListFromDB()
        {
            return _supplierInfoDataAccess.GetList<AlibabaSupplierInfo>();
        }

        public void InsertSupplierInfo(AlibabaSupplierInfo supplierInfo)
        {
            _supplierInfoDataAccess.Insert(supplierInfo);
        }

        public void UpdateSupplierInfo(AlibabaSupplierInfo supplierInfo)
        {
            _supplierInfoDataAccess.Update(supplierInfo);
        }

        public AlibabaSupplierInfo GetSupplierInfo(string id)
        {
            return _supplierInfoDataAccess.Get<AlibabaSupplierInfo>(id);
        }

        #endregion

    }
}
