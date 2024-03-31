using System;

namespace Ogu.Dal.Abstractions
{
    public interface IBaseEntity<TId> where TId : IEquatable<TId>
    {
        TId Id { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime? UpdatedOn { get; set; }
    }
}