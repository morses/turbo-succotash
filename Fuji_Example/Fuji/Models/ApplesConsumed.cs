using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fuji.Models
{
    [Table("ApplesConsumed")]
    public partial class ApplesConsumed
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("FujiUserID")]
        public int FujiUserId { get; set; }
        [Column("AppleID")]
        public int AppleId { get; set; }
        public int Count { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ConsumedAt { get; set; }

        [ForeignKey(nameof(AppleId))]
        [InverseProperty("ApplesConsumeds")]
        public virtual Apple Apple { get; set; } = null!;
        [ForeignKey(nameof(FujiUserId))]
        [InverseProperty("ApplesConsumeds")]
        public virtual FujiUser FujiUser { get; set; } = null!;
    }
}
