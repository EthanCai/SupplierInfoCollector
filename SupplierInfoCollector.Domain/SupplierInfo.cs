using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace SupplierInfoCollector.Domain
{
    public class SupplierInfo : BaseMongoDBModel
    {
        public override string CollectionNameInMongo
        {
            get { return "supplier_info"; }
        }

        /// <summary>
        /// Global Sources网站上的ID
        /// </summary>
        public string GlobalSourcesId { get; set; }

        public string Name { get; set; }

        public string Ranking { get; set; }

        public int YearsSince { get; set; }

        public int ProductCount { get; set; }

        /// <summary>
        /// Global Sources网站上的URL
        /// </summary>
        public string GlobalSourcesHomePageURL { get; set; }
        
        /// <summary>
        /// Global Sources网站上的产品列表页URL
        /// </summary>
        public string FullCatalogPageURL { get; set; }

        /// <summary>
        /// 公司网站主页URL
        /// </summary>
        public string CompageHomePageURL { get; set; }

        public string Contactor { get; set; }

        public string ContactorTitle { get; set; }

        public string ContactorEmail { get; set; }

        /// <summary>
        /// 邮箱图片的URL
        /// </summary>
        public string ContactorEmailImgURL { get; set; }


        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string StateOrProvince { get; set; }

        public string PostCode { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        
        public string MobilePhone { get; set; }

        public string CompanyNameZh { get; set; }

        public string AddressZh { get; set; }

        public string SupplierType { get; set; }

        public void AddSupplierType(string type)
        {
            if (string.IsNullOrEmpty(SupplierType))
            {
                this.SupplierType = type + ";";
            }
            else
            {
                string[] a1 = SupplierType.Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);
                if (!a1.Contains(type))
                {
                    this.SupplierType += (type + ";");
                }
            }
        }
    }
}
