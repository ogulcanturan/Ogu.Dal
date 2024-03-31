using System;

namespace Ogu.Dal.MongoDb.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoDatabaseAttribute : Attribute
    {
        public readonly string Table;
        public readonly string Database;
        public MongoDatabaseAttribute(string database = null, string table = null) 
        {
            Table = table;
            Database = database;
        }
    }
}