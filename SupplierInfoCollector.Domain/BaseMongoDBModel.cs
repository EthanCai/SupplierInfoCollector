using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupplierInfoCollector.Domain
{
    public class BaseMongoDBModel
    {
        /// <summary>
        /// MongoDB中的Id
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Mongo中的Collections名称
        /// </summary>
        public virtual string CollectionNameInMongo
        {
            get { return this.GetType().Name; }
        }
    }
}
