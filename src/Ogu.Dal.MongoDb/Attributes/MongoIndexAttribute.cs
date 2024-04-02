using Ogu.Dal.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ogu.Dal.MongoDb.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class MongoIndexAttribute : Attribute
    {
        public IReadOnlyDictionary<string, IndexTypeEnum> PropertyNameToIndexTypeDictionary { get; }
        public bool IsUnique { get; set; }

        public MongoIndexAttribute(string[] propertyNames, params IndexTypeEnum[] indexTypes)
        {
            if (propertyNames == null)
                throw new ArgumentNullException(nameof(propertyNames));

            if (propertyNames.Length == 0 || propertyNames.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Collection argument has empty elements.");
            }

            var dictionary = new Dictionary<string, IndexTypeEnum>(propertyNames.Length);

            if (propertyNames.Length > 1)
            {
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    dictionary[propertyNames[i]] = 
                        indexTypes == null || (i > indexTypes.Length - 1) ? IndexTypeEnum.Ascending : indexTypes[i];
                }
            }
            else
            {
                dictionary[propertyNames[0]] = indexTypes?.Length > 0 ? indexTypes[0] : IndexTypeEnum.Ascending;
            }

            PropertyNameToIndexTypeDictionary = dictionary;
        }
    }
}