using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace SupplierInfoCollector.Domain
{
    public class AlibabaSupplierInfo : BaseMongoDBModel
    {
        public override string CollectionNameInMongo
        {
            get { return "alibaba_supplier_info"; }
        }


        public override string Id
        {
            get { return "supplier_info_" + this.MemberID; }
            set {  }
        }

        public string MemberID { get; set; }

        public string AlibabaHomeURL { get; set; }


        public string CompanyName { get; set; }

        public string CompanyPhone { get; set; }

        public string MobilePhone { get; set; }

        public string CompanyFax { get; set; }

        public string CompanyAddress { get; set; }

        public double YearsOnAlibaba { get; set; }

        public string SupplierRank { get; set; }

        public string BusinessRating { get; set; }

        public string Contractor { get; set; }

        public string ContractorTitle { get; set; }

        public string WebsiteURL { get; set; }

        /// <summary>
        /// 提供的产品类目
        /// </summary>
        public string ProductTypes { get; set; }

        public void AddProductType(AlibabaProductType productType)
        {
            string str = productType.Id + "," + productType.Name;

            if (string.IsNullOrEmpty(ProductTypes))
            {
                ProductTypes = str + ";";
            }
            else
            {
                if (!ProductTypes.Contains(str))
                {
                    ProductTypes += (str + ";");
                }
            }
        }
    }
}
