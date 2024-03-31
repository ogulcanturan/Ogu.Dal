using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Ogu.Dal.Sql.Entities
{
    public abstract class EnumDatabase<TEnum> : BaseEntity<TEnum> where TEnum : struct, Enum, IEquatable<TEnum>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new TEnum Id { get; set; }
        protected EnumDatabase() { }
        protected EnumDatabase(TEnum id)
        {
            Id = id;
            Code = Id.ToString();
            Description = typeof(TEnum).GetField(id.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
        }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsEnumTypeExistsInProgram { get; set; } = true;
    }
}