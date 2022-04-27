using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fuji.Models
{
    [Table("FujiUser")]
    public partial class FujiUser
    {
        public FujiUser()
        {
            ApplesConsumeds = new HashSet<ApplesConsumed>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ASPNetIdentityId")]
        [StringLength(450)]
        public string AspnetIdentityId { get; set; } = null!;
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [InverseProperty(nameof(ApplesConsumed.FujiUser))]
        public virtual ICollection<ApplesConsumed> ApplesConsumeds { get; set; }
    }
}
