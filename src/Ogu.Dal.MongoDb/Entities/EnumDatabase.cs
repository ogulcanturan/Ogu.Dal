using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Ogu.Dal.Abstractions;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Ogu.Dal.MongoDb.Entities
{
    public abstract class EnumDatabase<TEnum> : IBaseEntity<TEnum> where TEnum : struct, Enum
    {
        protected EnumDatabase() { }
        protected EnumDatabase(TEnum id)
        {
            Id = id;
            Code = Id.ToString();
            Description = typeof(TEnum).GetField(id.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
        }

        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string BsonId { get; set; }
        public new TEnum Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsEnumValueExistsInProgram { get; set; } = true;
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}