﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Ogu.Dal.MongoDb.Entities
{
    public abstract class BaseEntity
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public virtual DateTime CreatedOn { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public virtual DateTime? UpdatedOn { get; set; }
    }
}