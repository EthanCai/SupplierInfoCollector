using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupplierInfoCollector.Domain;
using Nana.Framework.Utility.MongoDBHelper;

namespace SupplierInfoCollector.DataAccess
{
    public class SupplierInfoMongoDBDataAccess : BaseDataAccess
    {
        public List<SupplierInfo> GetSupplierInfoList()
        {
            return this.GetList<SupplierInfo>();
        }

        public void InsertSupplierInfo(SupplierInfo supplierInfo)
        {
            supplierInfo.Id = "supplier_info_" + supplierInfo.GlobalSourcesId;
            this.Insert(supplierInfo);
        }

        public void UpdateSupplierInfo(SupplierInfo supplierInfo)
        {
            this.Update(supplierInfo);
        }

        public SupplierInfo GetSupplierInfo(string globalSoucesId)
        {
            string id = "supplier_info_" + globalSoucesId;
            return this.Get<SupplierInfo>(id);
        }
    }
}
