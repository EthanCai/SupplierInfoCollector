using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupplierInfoCollector.DataAccess;
using SupplierInfoCollector.Domain;
using SupplierInfoCollector.WebPageUtils;

namespace SupplierInfoCollector.Business
{
    public class AlibabaProductTypeBusiness : BaseBusiness
    {
        private readonly AlibabaProductTypeCrawler _crawler = new AlibabaProductTypeCrawler();
        private readonly AlibabaProductTypeMongoDBDataAccess _productTypeDataAccess =
            new AlibabaProductTypeMongoDBDataAccess();

        #region alibaba product type

        public List<AlibabaProductType> GetProductTypeListFromWeb()
        {
            return _crawler.GetAlibabaProductTypes();
        }

        public List<AlibabaProductType> GetProductTypeListFromDB()
        {
            return _productTypeDataAccess.GetList<AlibabaProductType>();
        }

        public void InsertProductType(AlibabaProductType productType)
        {
            _productTypeDataAccess.Insert(productType);
        }

        public void UpdateProductType(AlibabaProductType productType)
        {
            _productTypeDataAccess.Update(productType);
        }

        public AlibabaProductType GetProductType(string id)
        {
            return _productTypeDataAccess.Get<AlibabaProductType>(id);
        }

        #endregion
    }
}
