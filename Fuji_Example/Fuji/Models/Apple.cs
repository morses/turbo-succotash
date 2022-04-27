using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fuji.Models
{
    [Table("Apple")]
    public partial class Apple
    {
        public Apple()
        {
            ApplesConsumeds = new HashSet<ApplesConsumed>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [StringLength(50)]
        public string VarietyName { get; set; } = null!;
        [StringLength(100)]
        public string? ScientificName { get; set; }

        [InverseProperty(nameof(ApplesConsumed.Apple))]
        public virtual ICollection<ApplesConsumed> ApplesConsumeds { get; set; }
    }
}
