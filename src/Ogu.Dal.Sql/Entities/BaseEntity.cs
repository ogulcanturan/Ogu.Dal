using Ogu.Dal.Abstractions;
using System;

namespace Ogu.Dal.Sql.Entities
{
    public abstract class BaseEntity<TId> : IBaseEntity<TId>
    {
        public virtual TId Id { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
    }
}