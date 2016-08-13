using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Builders;
using SupplierInfoCollector.Domain;
using Nana.Framework.Utility.MongoDBHelper;

namespace SupplierInfoCollector.DataAccess
{
    public class BaseDataAccess
    {
        public virtual List<T> GetList<T>() where T : BaseMongoDBModel
        {
            T t = Activator.CreateInstance<T>();
            var result = MongoDBHelper.GetAll<T>(t.CollectionNameInMongo);
            return result;
        }

        public virtual void Insert<T>(T t) where T : BaseMongoDBModel
        {
            var result = MongoDBHelper.InsertOne(t.CollectionNameInMongo, t);
        }

        public virtual void Update<T>(T t) where T : BaseMongoDBModel
        {
            var result = MongoDBHelper.UpdateOne(t.CollectionNameInMongo, t);
        }

        public virtual T Get<T>(string id) where T : BaseMongoDBModel
        {
            T t = Activator.CreateInstance<T>();
            var query = Query.EQ("_id", id);
            var result = MongoDBHelper.GetOne<T>(t.CollectionNameInMongo, query);
            return result;
        }
    }
}
