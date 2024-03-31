using System;
using System.Collections.Generic;

namespace Ogu.Dal.MongoDb.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoIndexAttribute : Attribute
    {
        public IReadOnlyList<string> PropertyNames { get; }
        public readonly bool IsUnique;
        public MongoIndexAttribute(bool isUnique = false, params string[] propertyNames)
        {
            PropertyNames = propertyNames;
            IsUnique = isUnique;
        }
    }
}