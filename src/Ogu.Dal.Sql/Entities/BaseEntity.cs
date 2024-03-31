using System;
using Ogu.Dal.Abstractions;

namespace Ogu.Dal.Sql.Entities
{
    public abstract class BaseEntity<TId> : IBaseEntity<TId> where TId : IEquatable<TId>
    {
        public virtual TId Id { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
    }
}