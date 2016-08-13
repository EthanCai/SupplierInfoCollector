using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace SupplierInfoCollector.Domain
{
    public class AlibabaProductType : BaseMongoDBModel
    {
        public override string CollectionNameInMongo
        {
            get { return "alibaba_product_type"; }
        }

        public override string Id
        {
            get { return string.IsNullOrEmpty(this.Name) ? null : "product_type_" + this.ChineseToAcsii(this.Name); }
            set { }
        }

        public string Name { get; set; }

        /// <summary>
        /// 比如 /家纺家装/家纺市场/四件套
        /// </summary>
        public string CategoryPath { get; set; }
        
        public string ListPageURL { get; set; }


        private string ChineseToAcsii(string zhString)
        {
            byte[] bytes = Encoding.GetEncoding("GBK").GetBytes(zhString);
            string result = BitConverter.ToString(bytes).Replace("-","");
            return result;
        }
    }
}
